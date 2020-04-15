const storage = window.localStorage;

const URL = "ws://localhost:8000";
const PROTOCOLS = "protocolOne";
const JOYSTICK = document.getElementById("joystick");
const STICK = document.getElementById("stick");
const JRADIUS = 150;
const STICKRADIUS = 50;
const HELP = document.getElementById("help");

const ws = new WebSocket(URL, PROTOCOLS);

let self = null;
let mygame = null;

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

// Get cursor position relative to joystick div
const getPosition = e => {
  let relX;
  let relY;
  const rect = JOYSTICK.getBoundingClientRect();
  // For handling mobile
  if (e.clientX == undefined) {
    // relY = (e.touches[0].clientY - rect.top) / rect.height;
    // relX = (e.touches[0].clientX - rect.left) / rect.width;
    // relY = e.touches[0].clientY - rect.top - STICKRADIUS + 1;
    // relX = e.touches[0].clientX - rect.left - STICKRADIUS + 1;
    relY = e.touches[0].clientY - rect.top;
    relX = e.touches[0].clientX - rect.left;
  } else {
    // relY = (e.clientY - rect.top) / rect.height;
    // relX = (e.clientX - rect.left) / rect.width;
    // relX = e.clientX - rect.left - STICKRADIUS + 1;
    // relY = e.clientY - rect.top - STICKRADIUS + 1;
    relX = e.clientX - rect.left;
    relY = e.clientY - rect.top;
  }

  return { x: relX, y: relY };
};
// Turn relative position of the joystick to vector
const getVector = (x, y) => {
  const vx = x - JRADIUS;
  const vy = -y + JRADIUS;
  const mag = Math.sqrt(vx * vx + vy * vy);
  const strength = mag / JRADIUS;
  return { x: vx / mag, y: vy / mag, strength: strength };
  // return { x: vx, y: vy, strength: 0 };
};

const resetJoystickPos = () => {
  STICK.style.top = `calc(var(--jradius) - var(--stickradius) + 1px)`;
  STICK.style.left = `calc(var(--jradius) - var(--stickradius) + 1px)`;
  // document.documentElement.style.setProperty(
  //   "--jx",
  //   `calc(var(--jradius) - var(--stickradius) + 1px)`,
  // );
  // document.documentElement.style.setProperty(
  //   "--jy",
  //   `calc(var(--jradius) - var(--stickradius) + 1px)`,
  // );
};
const setJoystickPos = (x, y) => {
  STICK.style.top = `calc(${y}px - var(--stickradius) + 1px)`;
  STICK.style.left = `calc(${x}px - var(--stickradius) + 1px)`;
  // STICK.style.top = `${y}px`;
  // STICK.style.left = `${x}px`;
  // document.documentElement.style.setProperty("--jx", `${x}px`);
  // document.documentElement.style.setProperty("--jy", `${y}px`);
  // document.documentElement.style.setProperty(
  //   "--jx",
  //   `calc(${x}px - var(--stickradius) + 1px)`,
  // );
  // document.documentElement.style.setProperty(
  //   "--jy",
  //   `calc(${y}px - var(--stickradius) + 1px)`,
  // );
};

const mouseup = () => {
  resetJoystickPos();
};
const onmousemove = e => {
  // If pressed down
  if (e.buttons === 1 || (e.clientX === undefined && e.touches)) {
    const { x, y } = getPosition(e);
    setJoystickPos(x, y);
    sendCoord(x, y);
  }
};

document.documentElement.style.setProperty("--jradius", `${JRADIUS}px`);
document.documentElement.style.setProperty("--stickradius", `${STICKRADIUS}px`);
document.addEventListener("mouseup", mouseup);
document.addEventListener("touchend", mouseup);
JOYSTICK.addEventListener("mousedown", onmousemove);
JOYSTICK.addEventListener("mousemove", onmousemove);
JOYSTICK.addEventListener("touchstart", onmousemove);
JOYSTICK.addEventListener("touchmove", e => {
  e.preventDefault();
  onmousemove(e);
});

// Socket Helpers
const registerSelf = () => {
  const userId = generatePlayerUUID();
  self = userId;
  ws.send(`WEB|${self}|NEW`);
};

const joinGame = () => {
  const gameId = document.getElementById("gameId").value;
  mygame = gameId;
  ws.send(`WEB|${self}|JOIN|${gameId}`);
};

const sendCoord = (x, y) => {
  const v = getVector(x, y);
  if (self && mygame) {
    ws.send(`WEB|${self}|SHOOT|${mygame}|${v.x} ${v.y} ${v.strength}`);
  }
};

ws.onopen = () => {
  registerSelf();
};

ws.onmessage = m => {
  const [r, g, b] = String(m.data).split(",");
  JOYSTICK.style.backgroundColor = `rgb(${r * 255}, ${g * 255}, ${b * 255})`;
  const luma = 0.2126 * r * 255 + 0.7152 * g * 255 + 0.0722 * b * 255;
  if (luma < 79) {
    JOYSTICK.style.setProperty("--joy-color", "white");
  } else {
    JOYSTICK.style.setProperty("--joy-color", "black");
  }
};

// TODO: this is broken
ws.onclose = () => {
  ws.send(`WEB|${self}|LEAVE`);
};
