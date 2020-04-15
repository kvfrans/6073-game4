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

var connected = 0;

// Socket Handlers


app.ws("/", (ws, req) => {
  wsClients.push(ws);

  // Everything comes to WS as a message
  ws.on("message", rawMsg => {
    console.log(`Received: ${rawMsg}`);
    const [command,who,data] = String(rawMsg).split("|");
    if (command === "JOIN") {
      connected += 1;
      ws.send("CONFIRM|"+who+"|"+connected);
      console.log("Client with uid %s now has id number %s", who, connected);
    } 
    else if (command === "DRAW") {
      ws.send(command);
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
