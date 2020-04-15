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

// Socket Handlers
app.ws("/", (ws, req) => {
  wsClients.push(ws);

  // Everything comes to WS as a message
  ws.on("message", rawMsg => {
    console.log(`Received: ${rawMsg}`);

    const [who, whoId, what, whomst, deets] = String(rawMsg).split("|");

    // if (what === "NEW") {
    //   directory[whoId] = ws;
    //   openSockets++;
    //   console.log("New socket registered");
    // } else if (what === "LEAVE") {
    //   directory[whoId] = null;
    //   openSockets--;
    //   console.log("Socket closed");
    // }
    //
    // if (who === "WEB") {
    //   if (what === "JOIN" && games.has(whomst)) {
    //     directory[whomst].send(`JOIN|${whoId}`);
    //   } else if (what === "SHOOT" && games.has(whomst)) {
    //     directory[whomst].send(`SHOOT|${whoId}|${deets}`);
    //   }
    // } else if (who === "UNITY") {
    //   if (what === "NEW") {
    //     games.add(whoId);
    //   } else if (what === "CLOSE") {
    //     games.delete(whoId);
    //   } else if (what === "COLOR" && directory[whomst]) {
    //     directory[whomst].send(`${deets}`);
    //   }
    // }
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
