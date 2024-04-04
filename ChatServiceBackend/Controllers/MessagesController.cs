using Microsoft.AspNetCore.Mvc;

namespace ChatServiceBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private static readonly List<string> Messages = new List<string>();

    public MessagesController()
    {
        // En un escenario real, aquí podrías suscribirte a Kafka y añadir mensajes a la lista.
    }

    [HttpGet]
    public IActionResult Get()
    {
        // Retorna todos los mensajes. En una aplicación real, querrías implementar lógica para solo retornar los nuevos mensajes desde la última solicitud.
        return Ok(Messages);
    }

    // Endpoint para simular la recepción de un nuevo mensaje desde Kafka
    [HttpPost]
    public IActionResult Post([FromBody] string message)
    {
        Messages.Add(message);
        return Ok();
    }
}
