using Discord;
using Discord.Interactions;

namespace ManfredHorst.Modules.Modal
{
    public class GeizhalsAddUrlModal : IModal
    {
        public String Title => "Geizhals Add Url Modal";

        [InputLabel("Geizhals Url")]
        [ModalTextInput("url", TextInputStyle.Short, placeholder: "Add the Url for the Geizhals items you want to track.", maxLength: 255)]
        public String Url { get; set; }
        
        [InputLabel("Name for the Alarm")]
        [ModalTextInput("alarm_name", TextInputStyle.Short, placeholder: "Cpu, Ram...", maxLength: 16)]
        public String Alias { get; set; }
    }
}