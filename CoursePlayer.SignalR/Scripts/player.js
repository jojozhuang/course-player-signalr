function playCourse(hub, playerHub, playstate, btnplay, processbar, currenttime, videoplayer, workingss, ss, workingwb, wb) {
    if (playstate == 'stopped') {
        hub.start().done(function () {
            playerHub.server.joinGroup($("#groupName").val());
            interval = setInterval(function () {
                processbar.slider("value", processbar.slider("value") + 1);
                currenttime.val(getReadableTimeText(processbar.slider("value")));
                playerHub.server.updateTime($("#groupName").val(), $("#processbar").slider("value"));
            }, 1000);
            btnplay.prop('value', 'Stop');
            playstate = "playing";
            if (videoplayer) { 
                console.log('play video go')
                videoplayer.play();
            }
        });
    } else if (playstate == 'playing') {
        hub.stop($("#groupName").val());
        processbar.slider("value", 0);
        currenttime.val(getReadableTimeText(processbar.slider("value")));
        playstate = "stopped";
        // stop the interval
        clearInterval(interval);
        // clear screenshot and whiteboard
        clearScreenshot(workingss, ss);
        clearWhiteboard(workingwb, wb);
        btnplay.prop('value', 'Play');
        if (videoplayer) {
            videoplayer.currentTime(0);
            videoplayer.pause();
        }
    }
    return playstate;
}

function drawScreenshot(ssdata, workingss, ss) {
    var left, top, width, height = 0;
    var imageList = JSON.parse(ssdata);

    console.log(imageList.length);
    for (var i = 0; i < imageList.length; i++) {
        left = workingss.width() / 8 * imageList[i].Col;
        top = workingss.height() / 8 * imageList[i].Row;
        width = workingss.width() / 8;
        height = workingss.height() / 8;
        drawImageOnCanvas(workingss, left, top, width, height, "data:image/png;base64," + imageList[i].ImageStream);
    }
    // draw entire working canvas to screenshot canvas
    var ctxss = ss[0].getContext('2d')
    ctxss.drawImage(workingss[0], 0, 0);
}

function drawImageOnCanvas(workingss, left, top, width, height, image) {
    var ctx = workingss[0].getContext("2d");
    var img = new Image();
    img.onload = function () {
        ctx.drawImage(img, left, top, width, height);
    }
    img.src = image;
}

function drawWhiteboard(wbdata, workingwb, wb) {
    var lastPoint;
    var currentColor = -10;
    var currentWidth = 1;
    var ctxwb = workingwb[0].getContext('2d');
    var xRate = workingwb.width() / 9600;
    var yRate = workingwb.height() / 4800;
    var wbobj = JSON.parse(wbdata);
    if (wbobj.WBLines) {
        for (var i = 0; i < wbobj.WBLines.length; i++) {
            var line = wbobj.WBLines[i];
            drawLine(ctxwb, getColor(line.Color), getWidth(line.Color), line.X0, line.Y0, line.X1, line.Y1, xRate, yRate);
        }
        var mywb = wb[0].getContext('2d');
        mywb.drawImage(workingwb[0], 0, 0);
    }
    if (wbobj.WBEvents) {
        var endMilliseconds = wbobj.Second * 1000 % 60000;
        for (var i = 0; i < endMilliseconds; i++) {
            for (var j = 0; j < wbobj.WBEvents.length; j++) {
                var event = wbobj.WBEvents[j];
                if (event && event.TimeStamp == i) {
                    if (event.X >= 0) {
                        if (!lastPoint) {
                            lastPoint = event;
                        } else {
                            drawLine(ctxwb, getColor(currentColor), currentWidth, lastPoint.X, lastPoint.Y, event.X, event.Y, xRate, yRate);
                            lastPoint = event;
                        }
                    } else {
                        switch (event.X) {
                            case -100: //Pen Up
                                currentColor = -8;
                                lastPoint = null;
                                break;
                            case -200: //Clear event
                                clearWhiteboard();
                                lastPoint = null;
                                break;
                            default:
                                currentColor = event.X;
                                currentWidth = getWidth(currentColor);
                                break;
                        }
                        lastPoint = null;
                    }
                }
            }
        }
        var mywb = wb[0].getContext('2d');
        mywb.drawImage(workingwb[0], 0, 0);
    }
}

function drawLine(ctxwb, color, width, x0, y0, x1, y1, xRate, yRate) {
    ctxwb.fillStyle = "solid";
    ctxwb.beginPath();
    ctxwb.strokeStyle = color;
    ctxwb.lineWidth = width;
    ctxwb.moveTo(x0 * xRate, y0 * yRate);
    ctxwb.lineTo(x1 * xRate, y1 * yRate);
    ctxwb.closePath();
    ctxwb.stroke();
}

function getColor(color) {
    switch (color) {
        case -1:
            return '#FF0000';
        case -2:
            return '#0000FF';
        case -3:
            return '#00FF00';
        case -8:
            return '#000000';
        case -9:
            return '#FFFFFF';
        case -10:
            return '#FFFFFF';
        default:
            return '#FFFFFF';
    }
}

function getWidth(color) {
    switch (color) {
        case -1:
            return 1;
        case -2:
            return 1;
        case -3:
            return 1;
        case -8:
            return 1;
        case -9:
            return 8 * 10 / 12;
        case -10:
            return 39 * 10 / 12;
        default:
            return 1;
    }
}

function clearScreenshot(workingss, ss) {
    // reset screen
    var ctxworkingss = workingss[0].getContext('2d');
    ctxworkingss.clearRect(0, 0, workingss.width(), workingss.height());
    var ctxss = ss[0].getContext('2d');
    ctxss.clearRect(0, 0, ss.width(), ss.height());
}

function clearWhiteboard(workingwb, wb) {
    // reset whiteboard
    var ctxworkingwb = workingwb[0].getContext('2d');
    ctxworkingwb.clearRect(0, 0, workingwb.width(), workingwb.height());
    var ctxwb = wb[0].getContext('2d');
    ctxwb.clearRect(0, 0, wb.width(), wb.height());
}

function getReadableTimeText(totalseconds) {
    var hours, minutes, seconds = 0;
    seconds = totalseconds % 60;
    hours = Math.floor(totalseconds / (60 * 60));
    minutes = Math.floor((totalseconds - hours * 60 * 60) / 60);

    var outh, outm, outs = "";
    outh = hours < 10 ? "0" + hours : hours;
    outm = minutes < 10 ? "0" + minutes : minutes;
    outs = seconds < 10 ? "0" + seconds : seconds;

    return outh + ":" + outm + ":" + outs;
}