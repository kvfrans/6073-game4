// Utility functions

/**
 * Generates random 6 character string for UID
 * Used for game codes
 * credit: https://stackoverflow.com/questions/6248666/how-to-generate-short-uid-like-ax4j9z-in-js
 */
const generateUID = () => {
  var firstPart = (Math.random() * 46656) | 0;
  var secondPart = (Math.random() * 46656) | 0;
  firstPart = ("000" + firstPart.toString(36)).slice(-3);
  secondPart = ("000" + secondPart.toString(36)).slice(-3);
  return firstPart + secondPart;
};

/**
 * Function that generates a unique player id based on time
 * Credit: https://gist.github.com/gordonbrander/2230317
 * @return {string}
 */
const generatePlayerUUID = () => {
  let uuid = (
    Date.now().toString(36) +
    Math.random()
      .toString(36)
      .substr(2, 5)
  ).toUpperCase();
  return uuid;
};

/**
 * @param {Array}
 * @returns item in array
 */
const randFrom = array => {
  let rand = Array.from(array)[Math.floor(Math.random() * array.size)];
  return rand;
};

/**
 * @param {number} t in milliseconds
 * @param {function} fn to be executed
 * Credit: https://stackoverflow.com/questions/8126466/how-do-i-reset-the-setinterval-timer
 * Credit: https://stackoverflow.com/questions/20618355/the-simplest-possible-javascript-countdown-timer
 */
function Timer(fn, t) {
  var t, minutes, seconds;
  var timerObj = setInterval(function() {
    if (--t < 0) {
      t = 0;
    }
    fn();
  }, 1000);

  this.parseTime = function() {
    if (timerObj) {
      minutes = parseInt(t / 60, 10);
      seconds = parseInt(t % 60, 10);

      minutes = minutes < 10 ? "0" + minutes : minutes;
      seconds = seconds < 10 ? "0" + seconds : seconds;

      return `${minutes}:${seconds}`;
    } else {
      return "00:00";
    }
  };

  this.stop = function() {
    if (timerObj) {
      clearInterval(timerObj);
      timerObj = null;
    }
    return this;
  };

  // start timer using current settings (if it's not already running)
  this.start = function() {
    if (!timerObj) {
      this.stop();
      timerObj = setInterval(fn, 1000);
    }
    return this;
  };

  // start with new time, stop current interval
  this.reset = function(newT) {
    t = newT;
    return this.stop().start();
  };
}

/**
 * @param {number} t in milliseconds
 * @param {function} callback
 */
function Countdown(fn, t) {
  let timer = setTimeout(fn, t);

  this.start = () => {
    if (!timer) {
      timer = setTimeout(fn, t);
    }
  };

  this.stop = () => {
    if (timer) {
      clearTimeout(timer);
    }
    timer = null;
  };
}

/****************
 * State class  *
 ***************/
class State {
  constructor() {
    this.state = 0; // 0 is lobby
    this.players = {};
    this.gameTime = null; // countdown for 7 minutes
  }
  // Game player functions
  add(uuid, name) {
    let player = new Caster(uuid, name);
    this.players[uuid] = player;
    return uuid;
  }
  kick(uuid) {
    delete this.players[uuid];
  }
  has(uuid) {
    return Object.keys(this.players).includes(uuid);
  }
  // Game state functions
  start() {
    this.advanceState();

    // Game timer started in app.js
  }
  // GET functions
  get playerCount() {
    return Object.keys(this.players).length;
  }
  get playerList() {
    let cast = [];
    for (let p of Object.keys(this.players)) {
      cast.push(this.players[p].name);
    }
    return cast;
  }
  get playerIDs() {
    return Object.keys(this.players);
  }
  get time() {
    return this.gameTime.parseTime();
  }
  // Helper functions
  advanceState() {
    this.state++;
  }
}

// Export
module.exports = {
  generateUID: generateUID,
  generatePlayerUUID: generatePlayerUUID,
  randFrom: randFrom,
  Timer: Timer,
  Countdown: Countdown,
  State: State,
};
