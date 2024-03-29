﻿using Discord;
using Discord.Interactions;

namespace ManfredHorst.Modules.Modal;

public class AddAlarmModal : IModal
{
    public String Title => "Add Alarm";

    [InputLabel("Geizhals Url")]
    [ModalTextInput("alarm_url", TextInputStyle.Short, placeholder: "Add the Url for the Geizhals items you want to track.", maxLength: 200)]
    public String Url { get; set; }

    [InputLabel("Name for the Alarm")]
    [ModalTextInput("alarm_name", TextInputStyle.Short, placeholder: "Cpu, Ram... Max 41 characters", maxLength: 41)]
    public String Alias { get; set; }

    [InputLabel("Price")]
    [ModalTextInput("alarm_price", TextInputStyle.Short, placeholder: "Enter your alarm price", maxLength: 6)]
    public Double Price { get; set; }
}