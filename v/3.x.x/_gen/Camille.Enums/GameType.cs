
// This file is automatically generated.
// Do not directly edit.
// Generated on 2021-04-30T02:32:07.806Z

using System.ComponentModel.DataAnnotations;

namespace Camille.Enums
{
    /// <summary>
    /// GameTypes enum based on gameTypes.json.
    /// </summary>
#if USE_NEWTONSOFT
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
#elif USE_SYSTEXTJSON
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
#endif
    public enum GameType
    {
        [Display(Name = "CUSTOM_GAME", Description = "Custom games")]
        CUSTOM_GAME,
        [Display(Name = "MATCHED_GAME", Description = "all other games")]
        MATCHED_GAME,
        [Display(Name = "TUTORIAL_GAME", Description = "Tutorial games")]
        TUTORIAL_GAME,
    }
}
