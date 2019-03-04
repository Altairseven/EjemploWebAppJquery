var express = require('express');

const hostname = 'localhost';
const port = 80;


//Servidor de express
var app = express();

app.use(express.static("www"));
app.listen(port, hostname, () => {
    console.log(`Server running at http://${hostname}:${port}/`);
  });