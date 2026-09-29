//HTTP 모듈 ㅗ딩

let http = require("http");

http.createServr(function (request, response)
{
    response.writeHead(200,{'Content-Type' : 'text/plain'})

    response.end("Hellow wolrd");

}).listen(8000);

console.log("Server running");