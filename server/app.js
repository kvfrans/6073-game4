const express = require("express");
const path = require("path");
const app = express();
const expressWS = require("express-ws")(app);

const GAME = require("./game.js");
const HELPER = require("./helper.js");

// CONSTANTS
const PORT = 8000;

// Setting up Views and Directories
app.set("views", path.join(__dirname, "views"));
app.set("view engine", "pug");
app.use("/static", express.static(path.join(__dirname, "public")));

// Open games
const openGames = {
  notFoo: new GAME.State(),
};

// DEBUG: purposes only
let openSockets = 0;

// { id: socket }
const directory = {};
const games = new Set();

const wsClients = [];
const players = [];

var connected = 0;

// Socket Handlers


app.ws("/", (ws, req) => {
  wsClients.push(ws);

  // Everything comes to WS as a message
  ws.on("message", rawMsg => {
    // console.log(`Received: ${rawMsg}`);
    const [command,who,data] = String(rawMsg).split("|");
    if (command === "JOIN") {
      if (players.length >= 4) // too many people
        return; // TODO!!
      connected += 1;
      ws.send("CONFIRM|" + who + "|" + connected);
      players.push(who);
      var playerStr = "";
      players.forEach((x) => playerStr += "_" + x);
      for(var i = 0; i < wsClients.length; i++) {
        wsClients[i].send("PLAYERS|" + playerStr.substring(1));
      }
      console.log("Client with uid %s now has id number %s", who, connected);
    }
    else if (command === "START") {
      for(var i = 0; i < wsClients.length; i++) {
        wsClients[i].send(String(rawMsg));
      }
    }
    else if (command === "DRAW") {
      for(var i = 0; i < wsClients.length; i++) {
        wsClients[i].send(String(rawMsg));
      }
    }
    else if (command === "CHEATER_GUESS") {
      console.log(String(rawMsg));
      for(var i = 0; i < wsClients.length; i++) {
        wsClients[i].send(String(rawMsg));
      }
    }
  });
});

// Routes
app.get("/", function(req, res) {
  res.render("index", { routeGame: false });
});
app.get("/:gamecode", function(req, res) {
  // Try game code
  let gamecode = req.params.gamecode;
  if (gamecode.length === 6) {
    if (gamecode in openGames) {
      res.render("index", { routeGame: gamecode });
    }
  }

  res.redirect("/");
});

// Run App
app.listen(PORT);
