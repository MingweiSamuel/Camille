using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Camille.Lcu
{
    public class AutomaticLockfileProvider : IDisposable, ILockfileProvider
    {
        private readonly TimeSpan _processPollingInterval;

        private readonly SemaphoreSlim _lockfilePathLock = new SemaphoreSlim(1);
        private string? _lockfilePath = null;

        private FileSystemWatcher? _watcher = null;

        /// <summary>
        /// This lock must be held to modify _lockfileTaskSource or _lockfileTaskSourceResolved.
        /// </summary>
        private readonly object _lockfileTaskSourceLock = new object();
        private TaskCompletionSource<Lockfile> _lockfileTaskSource = new TaskCompletionSource<Lockfile>();
        private bool _lockfileTaskSourceResolved = false;

        public AutomaticLockfileProvider() : this(new TimeSpan(0, 0, 1))
        { }

        public AutomaticLockfileProvider(TimeSpan processPollingInterval)
        {
            _processPollingInterval = processPollingInterval;
        }

        public AutomaticLockfileProvider(string lockfilePath)
        {
            _lockfilePath = lockfilePath;
        }

        public async Task<Lockfile> GetLockfile(CancellationToken token)
        {
            await LockfilePathFound(token);

            Task<Lockfile> lockfileTask;
            Task<Task> eitherTask;
            lock (_lockfileTaskSourceLock)
            {
                var cancellationTask = Task.Delay(Timeout.Infinite, token);

                lockfileTask = _lockfileTaskSource.Task;
                eitherTask = Task.WhenAny(cancellationTask, lockfileTask);
            }
            await eitherTask;

            token.ThrowIfCancellationRequested();
            return await lockfileTask;
        }

        private async Task LockfilePathFound(CancellationToken token)
        {
            if (null == _lockfilePath)
            {
                await _lockfilePathLock.WaitAsync(token);
                try
                {
                    // Find the lockfile path if it hasn't been found yet.
                    while (null == _lockfilePath)
                    {
                        try
                        {
                            _lockfilePath = Lockfile.GetLockfilePathFromProcess();
                            // See if LCU is already running.
                            CheckUpdatedLockfile();

                            // Set file watchers.
                            var path = Path.GetDirectoryName(_lockfilePath);
                            Debug.Assert(null != path);
                            _watcher = new FileSystemWatcher(path)
                            {
                                NotifyFilter = NotifyFilters.LastWrite,
                                Filter = Path.GetFileName(_lockfilePath)
                            };
                            _watcher.Changed += OnLockfileChanged;
                            _watcher.Created += OnLockfileChanged;
                            _watcher.Deleted += OnLockfileChanged;

                            _watcher.EnableRaisingEvents = true; // Start the watcher.

                            break;
                        }
                        catch (InvalidOperationException)
                        {
                            await Task.Delay(_processPollingInterval, token);
                        }
                    }
                }
                finally
                {
                    _lockfilePathLock.Release();
                }
            }
        }

        private void OnLockfileChanged(object sender, FileSystemEventArgs e)
        {
            CheckUpdatedLockfile();
        }

        private void CheckUpdatedLockfile()
        {
            lock (_lockfileTaskSourceLock)
            {
                Debug.Assert(null != _lockfilePath);

                Lockfile lockfile;
                try
                {
                    lockfile = Lockfile.ParseFile(_lockfilePath);
                }
                catch
                {
                    // Lockfile is not there.
                    if (_lockfileTaskSourceResolved)
                    {
                        // ...because it was removed, reset the task.
                        _lockfileTaskSource = new TaskCompletionSource<Lockfile>();
                    }
                    return;
                }

                // Lockfile is there.
                if (_lockfileTaskSourceResolved)
                {
                    // ...and was updated.
                    _lockfileTaskSource = new TaskCompletionSource<Lockfile>();
                }
                _lockfileTaskSource.SetResult(lockfile);
                _lockfileTaskSourceResolved = true;
            }
        }

        public void Dispose()
        {
            if (null != _watcher)
            {
                _watcher.Dispose();
            }
        }
    }
}
