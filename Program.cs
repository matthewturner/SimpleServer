using System.Net;
using System.Text;

var server = new HttpListener();
server.Prefixes.Add("http://127.0.0.1:8000/");
server.Prefixes.Add("http://localhost:8000/");

server.Start();

Console.WriteLine("Listening...");

while (true)
{
    var context = server.GetContext();
    var response = context.Response;

    var page = Path.Combine(Directory.GetCurrentDirectory(), context.Request.Url!.LocalPath.Substring(1));

    if (context.Request.Url!.LocalPath == "/")
    {
        page = Path.Combine(Directory.GetCurrentDirectory(), "index.html");
    }

    if (!File.Exists(page))
    {
        response.StatusCode = (int)HttpStatusCode.NotFound;
        context.Response.Close();
        continue;
    }

    var reader = new StreamReader(page);
    var msg = reader.ReadToEnd();

    var buffer = Encoding.UTF8.GetBytes(msg);

    response.ContentLength64 = buffer.Length;
    response.ContentType = "text/html";

    response.AddHeader("Referrer-Policy", "strict-origin-when-cross-origin");
    response.AddHeader("X-Content-Type-Options", "nosniff");
    response.AddHeader("X-Permitted-Cross-Domain-Policies", "none");
    response.AddHeader("X-Xss-Protection", "1; mode=block");
    response.AddHeader("Content-Security-Policy", "base-uri 'self'; block-all-mixed-content; child-src 'self'; connect-src 'self' default-src 'self'; font-src 'self' form-action 'none'; frame-ancestors 'none'; frame-src 'none'; img-src 'self' data: https://c0.froala.com; manifest-src 'self'; media-src 'none'; object-src 'none'; sandbox allow-scripts allow-same-origin allow-storage-access-by-user-activation allow-forms allow-popups allow-modals; script-src 'self'; script-src-attr 'unsafe-inline'; script-src-elem 'self' 'unsafe-inline' https://static.hotjar.com https://script.hotjar.com; style-src 'self'; style-src-attr 'unsafe-inline'; style-src-elem 'self' 'unsafe-inline' upgrade-insecure-requests; worker-src 'none';");

    response.OutputStream.Write(buffer, 0, buffer.Length);

    context.Response.Close();
}