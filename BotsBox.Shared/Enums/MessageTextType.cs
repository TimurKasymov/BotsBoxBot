using System.ComponentModel;

namespace BotsBox.Shared.Enums
{
    public enum MessageTextType
    {
        [Description("Отмена")]
        Cancel = 0,
        [Description("Вернуться")]
        Return = 1
    }
}