using Discord;
using Discord.Interactions;

namespace ManfredHorst.Modules.Modal
{
    public class DeleteAlarmModal : IModal
    {
        public String Title => "Delete Alarm";

        [InputLabel("Alarm Name")]
        [ModalTextInput("alarm_name", TextInputStyle.Short, placeholder: "Enter your Alarm Name", maxLength: 255)]
        public String Alias { get; set; }

    }
}