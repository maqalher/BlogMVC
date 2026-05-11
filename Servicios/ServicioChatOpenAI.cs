using System;
using BlogMVC.Configuraciones;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace BlogMVC.Servicios;

public class ServicioChatOpenAI: IServicioChat
{
    private readonly IOptions<ConfiguracionesIA> options;
    private readonly OpenAIClient openAIClient;
    private string systemPromptGenerarCuerpo = """
        Eres un ingeniero de software experto en ASP.NET Core.
        Escribes articulos con un tono jovial y amigable.
        Te esfuerzas para que los principiantes entiendad las cosas dando ejemplos practicos
     """;
    private string ObtenerPromptGenerarCuerpo(string titulo) => $"""
        Crear un articulo para un blog. El titulo del articulo sera {titulo}
        Si lo entiendes conveniente, debes insertar tipos.
        El formato de respuesta es HTML. Por tanto, debes colocar negritas donde consideres, titulo, subtitulos, entre otras cosas que ayuden a resaltar el formato.
        La respuesta no debe ser un documento HTML, sino solamente el articulo en formato HTML, con sus parrafos bien separados. Por tanto, nada de DOCTYPE, ni head, ni body. Solo el articulo.
        No incluyas el titulo del articulo en el articulo.
    """;

    public ServicioChatOpenAI(IOptions<ConfiguracionesIA> options, OpenAIClient openAIClient)
    {
        this.options = options;
        this.openAIClient = openAIClient;
    }

    public async Task<string> GenerarCuerpo(string titulo)
    {
        var modeloText = options.Value.ModeloTexto;
        var clienteChat = openAIClient.GetChatClient(modeloText);

        var mensajeDeSistema = new SystemChatMessage(systemPromptGenerarCuerpo);

        var pormpotUsuario = ObtenerPromptGenerarCuerpo(titulo);

        var mensajeUsuario = new UserChatMessage(pormpotUsuario);

        ChatMessage[] mensaje = { mensajeDeSistema, mensajeUsuario };
        var respuesta = await clienteChat.CompleteChatAsync(mensaje);
        var cuerpo = respuesta.Value.Content[0].Text;
        return cuerpo;
    }

    public async IAsyncEnumerable<string> GenerarCuerpoStream(string titulo)
    {
        var modeloText = options.Value.ModeloTexto;
        var clienteChat = openAIClient.GetChatClient(modeloText);

        var mensajeDeSistema = new SystemChatMessage(systemPromptGenerarCuerpo);

        var pormpotUsuario = ObtenerPromptGenerarCuerpo(titulo);

        var mensajeUsuario = new UserChatMessage(pormpotUsuario);
        ChatMessage[] mensajes = { mensajeDeSistema, mensajeUsuario };

        await foreach (var completionUpdate in clienteChat.CompleteChatStreamingAsync(mensajes))
        {
            foreach (var contenido in completionUpdate.ContentUpdate)
            {
                yield return contenido.Text;
            }
        }
    }

}
