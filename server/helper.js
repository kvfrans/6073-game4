// Websocket Helper functions

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

// Export
module.exports = {
  generateUID: generateUID,
  generatePlayerUUID: generatePlayerUUID,
};
