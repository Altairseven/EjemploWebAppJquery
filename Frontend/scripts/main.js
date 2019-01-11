/* Variables Globales */
const ApiURL = "https://localhost:44394/api";
const WebUrl = "http://localhost/TEST"

/* Imports Globales */

var script = document.createElement('link');
script.href = '../libs/css/bootstrap.css';
script.rel = 'stylesheet';
document.getElementsByTagName('head')[0].appendChild(script);

var script = document.createElement('link');
script.href="https://use.fontawesome.com/releases/v5.3.1/css/all.css";
script.rel = 'stylesheet';

script.crossorigin="anonymous";

document.getElementsByTagName('head')[0].appendChild(script);

var script = document.createElement('link');
script.href = '../libs/css/sweetalert2.min.css';
script.rel = 'stylesheet';
document.getElementsByTagName('head')[0].appendChild(script);

//Jquery
var script = document.createElement('script');
script.src = '../libs/js/jquery-3.3.1.min.js';
script.type = 'text/javascript';
document.getElementsByTagName('head')[0].appendChild(script);

//Popper
var script = document.createElement('script');
script.src = '../libs/js/popper.js';
script.type = 'text/javascript';
document.getElementsByTagName('head')[0].appendChild(script);
//Bootstrap
var script = document.createElement('script');
script.src = '../libs/js/bootstrap.js';
script.type = 'text/javascript';
document.getElementsByTagName('head')[0].appendChild(script);
//SweetAlert
var script = document.createElement('script');
script.src = '../libs/js/sweetalert2.min.js';
script.type = 'text/javascript';
document.getElementsByTagName('head')[0].appendChild(script);


/*Funciones Globales */

//Antecede el Url del Api a cualquier url que se le pase.
function _SetUrl(controller){
    let url = ApiURL;
    if(!controller)
        return url;
    else
        return url + "/" + controller;
}
//Procesa los datos de sesion.
function _processLoginInfo(response){
    debugger;
    if(!response){
        localStorage.removeItem('username');
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem('nombre');
        localStorage.removeItem('apellido');
        localStorage.removeItem('expiration');
        return false
    }
    localStorage.setItem('username', response.Username);
    localStorage.setItem('token', response.token);
    localStorage.setItem('userId', response.ID);
    localStorage.setItem('nombre', response.Nombre);
    localStorage.setItem('apellido', response.Apellido);
    localStorage.setItem('expiration', response.TokenExpiration);
    return true;
}
//Se fija si existe una sesion valida(sin expirar).
function _isLoggedIn(){

    if(!localStorage.getItem("expiration")){
        return false;
    }

    var expiration = new Date(localStorage.getItem("expiration"))
    if(new Date() > expiration){
        return false;
    }


    return true;
}


//los request que no necesiten una sesion deben usar este metodo.
function _setHeaderSinAuth(e){
    e.setRequestHeader('Access-Control-Allow-Origin', '*'); 
}
//Los que si requieren que la sesion exista, tienen que usar este.
function _setHeader(e){

    var expiration = new Date(localStorage.getItem("expiration"))
    if(new Date() > expiration){
        _SessionCaducada();
        return false;
    }

    e.setRequestHeader('Access-Control-Allow-Origin', '*');
    e.setRequestHeader('idUsuario', localStorage.getItem("userId"))
    e.setRequestHeader('Authorization', "Bearer " + localStorage.getItem("token"))
}
function _handle401(err){
    if(err.status == 401)
        _SessionCaducada();
}

function _SessionCaducada(){
    Swal({
        title: 'La sesion Expiro',
        type: 'error',
        confirmButtonColor: '#3085d6',
        confirmButtonText: 'Ok',
    
    }).then((result) => {
        if (result.value) { //Si se apreto que si:
            _killSession();
        }
    })

}
function _killSession(){
    localStorage.removeItem('username');
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('nombre');
    localStorage.removeItem('apellido');
    localStorage.removeItem('expiration');

    window.location = WebUrl;
}

//Sirve para obtener los parametros locales con los que se abrio la pagina actual en la que se esta ubicado.
//ej... usuariosForm.html?id=123  //si el id es 0 lo uso para registrar uno nuevo, sino, es para editar uno existente.
function GetParamsByCurrentUrl() {
    debugger
    var url = window.location.href;
    // get query string from url (optional) or window
    var queryString = url ? url.split('?')[1] : window.location.search.slice(1);

    // we'll store the parameters here
    var obj = {};

    // if query string exists
    if (queryString) {

    // stuff after # is not part of query string, so get rid of it
    queryString = queryString.split('#')[0];

    // split our query string into its component parts
    var arr = queryString.split('&');

    for (var i = 0; i < arr.length; i++) {
        // separate the keys and the values
        var a = arr[i].split('=');

        // set parameter name and value (use 'true' if empty)
        var paramName = a[0];
        var paramValue = typeof (a[1]) === 'undefined' ? true : a[1];

        // (optional) keep case consistent
        paramName = paramName.toLowerCase();
        if (typeof paramValue === 'string') paramValue = paramValue.toLowerCase();

        // if the paramName ends with square brackets, e.g. colors[] or colors[2]
        if (paramName.match(/\[(\d+)?\]$/)) {

        // create key if it doesn't exist
        var key = paramName.replace(/\[(\d+)?\]/, '');
        if (!obj[key]) obj[key] = [];

        // if it's an indexed array e.g. colors[2]
        if (paramName.match(/\[\d+\]$/)) {
            // get the index value and add the entry at the appropriate position
            var index = /\[(\d+)\]/.exec(paramName)[1];
            obj[key][index] = paramValue;
        } else {
            // otherwise add the value to the end of the array
            obj[key].push(paramValue);
        }
        } else {
        // we're dealing with a string
        if (!obj[paramName]) {
            // if it doesn't exist, create property
            obj[paramName] = paramValue;
        } else if (obj[paramName] && typeof obj[paramName] === 'string'){
            // if property does exist and it's a string, convert it to an array
            obj[paramName] = [obj[paramName]];
            obj[paramName].push(paramValue);
        } else {
            // otherwise add the property
            obj[paramName].push(paramValue);
        }
        }
    }
    }

    return obj;
}

function FeedSelect(url, Objeto, DefaultValue, AllowNone, NoneTxt){
    return new Promise((resolve,reject) =>{
        $.ajax({
            url: _SetUrl(url),
            method: "GET",
            beforeSend: _setHeader
        }).done(x=>{

            Objeto.html("");
            debugger;
            if(AllowNone){
                let opt = new Option();
                opt.value = 0;
                opt.innerHTML = NoneTxt;
                Objeto.append(opt);
            }

            for (const elem of x) {
                let opt = new Option();
                opt.value = elem.ID;
                opt.innerHTML = elem.Nombre;
                if(elem.ID == DefaultValue)
                    opt.selected = true;

                Objeto.append(opt);
            }
            resolve(x);
        }).fail(x=>{
            console.error("Error al Llenar el Select:", x);
            resolve();
        })






    })
}

function stringToBytes(str) {
    var ch, st, re = [];
    for (var i = 0; i < str.length; i++ ) {
      ch = str.charCodeAt(i);  // get char 
      st = [];                 // set up "stack"
      do {
        st.push( ch & 0xFF );  // push byte to stack
        ch = ch >> 8;          // shift value down by 1 byte
      }  
      while ( ch );
      // add stack contents to result
      // done because chars have "wrong" endianness
      re = re.concat( st.reverse() );
    }
    // return an array of bytes
    return re;
  }

function _downloadPdf(url, openInNewTab = false){
    let filename
    fetch(url, {
        method: 'GET',
        headers: new Headers({
            'Access-Control-Allow-Origin': '*',
            'idUsuario': localStorage.getItem("userId"),
            "Authorization": "Bearer " + localStorage.getItem("token"),
        })
    })
    .then(response => {
        debugger;
        filename = response.headers.get("filename");
        return response.blob()
    })
    .then(blob => {
        // debugger;
        var url = window.URL.createObjectURL(blob);
        if(openInNewTab){
            window.open(url,"_blank");
            return;
        }
        var a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a); // we need to append the element to the dom -> otherwise it will not work in firefox
        a.click();    
        a.remove();  //afterwards we remove the element again         
        


    });
}
function _downloadExcel(url){
    let filename
    fetch(url, {
        method: 'GET',
        headers: new Headers({
            'Access-Control-Allow-Origin': '*',
            'idUsuario': localStorage.getItem("userId"),
            "Authorization": "Bearer " + localStorage.getItem("token"),
        })
    })
    .then(response => {
        debugger;
        filename = response.headers.get("filename");
        return response.blob()
    })
    .then(blob => {
        // debugger;
        var url = window.URL.createObjectURL(blob);
        var a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a); // we need to append the element to the dom -> otherwise it will not work in firefox
        a.click();    
        a.remove();  //afterwards we remove the element again         

    });
}