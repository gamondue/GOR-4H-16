var WebSocketServer = require('ws').Server, //importo il tipo di oggetto Server
	wss = new WebSocketServer({port: 8080});
wss.on('connection', function(ws) {
    ws.on('message', function(message) {
        console.log('received: %s', message);
		ws.send('RISPOSTA DAL SERVER DEL PROF.');
    });
});