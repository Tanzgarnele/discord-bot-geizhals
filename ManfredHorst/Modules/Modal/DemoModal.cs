using Discord;
using Discord.Interactions;

namespace ManfredHorst.Modules.Modal;

public class DemoModal : IModal
{
    public String Title => "Demo Modal";

    [InputLabel("Send a greeting!")]
    [ModalTextInput("gretting input", TextInputStyle.Short, placeholder: "Add a new Url", maxLength: 255)]
    public String greeting { get; set; }
}