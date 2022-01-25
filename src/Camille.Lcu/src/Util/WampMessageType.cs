namespace Camille.Lcu.Util
{
    /// <summary>
    /// Enum of message type IDs for WAMP messages.
    /// </summary>
    enum WampMessageType : int
    {
        Welcome = 0,
        Prefix = 1,
        Call = 2,
        CallResult = 3,
        CallError = 4,
        Subscribe = 5,
        Unsubscribe = 6,
        Publish = 7,
        Event = 8
    }
}
