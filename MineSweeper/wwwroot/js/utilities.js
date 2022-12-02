/* Convert the face into a surprised face */
function faceSurprised(event) {
    if (event.button === 0) {
        document.getElementById("face").className = "face-surprised";
    }
}

/* Convert the face into a smily face */
function faceSmile() {
    var face = document.getElementById("face");

    if (face !== undefined)
        face.className = "face-smile";
}

/* Set the stop watch time */
function setTime(hundreds, tens, ones) {
    var hundredsElement = document.getElementById("seconds_hundreds");
    var tensElement = document.getElementById("seconds_tens");
    var onesElement = document.getElementById("seconds_ones");

    if (hundredsElement !== null) {
        hundredsElement.className = "time-" + hundreds;
    }
    if (tensElement !== null) {
        tensElement.className = "time-" + tens;
    }
    if (onesElement !== null) {
        onesElement.className = "time-" + ones;
    }
}