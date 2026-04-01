namespace Opplat.Domain.Models;

public class Notification
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public virtual ICollection<UserNotification> UserNotification { get; set; } = [];

    public string Created
    {
        get
        {
            var created = DateTime.Now - CreateDate;

            double dias = Convert.ToInt32(created.TotalDays);
            double horas = Convert.ToInt32(created.TotalHours);
            double minutos = Convert.ToInt32(created.TotalMinutes);
            double segundos = Convert.ToInt32(created.TotalSeconds);

            if (dias > 0)
            {
                return "hace " + dias.ToString() + " días";
            }
            else if (horas > 0)
            {
                return "hace " + horas.ToString() + " horas";
            }
            else if (minutos > 0)
            {
                return "hace " + minutos.ToString() + " minutos";
            }
            else
            {
                return "justo ahora";
            }
        }
    }

    public string Module { get; set; } = string.Empty;
}
