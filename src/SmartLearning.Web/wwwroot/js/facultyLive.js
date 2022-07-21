// Create SignalRConnection
'use strict';
let hubUrl = '/liveHub';
let signal = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl, signalR.HttpTransportType.WebSockets)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.None).build();

let peerConnectionConfig = {
    iceServers: [
        {
            urls: ["stun:stun.l.google.com:19302", "stun:stun1.l.google.com:19302", "stun:stun2.l.google.com:19302", "stun:stun3.l.google.com:19302", "stun:stun4.l.google.com:19302"]
        },
        {
            urls: "turn:numb.viagenie.ca",
            credential: 'muazkh',
            username: 'webrtc@live.com'
        }
    ]
};

let connections = {}, localStream = new MediaStream();
var webrtcConstraints = { audio: true, video: true };
let myvideo = document.getElementById('myvideo');
let cid = document.querySelector("#liveclass").dataset.cid;
let myid = document.querySelector("#liveclass").dataset.bid;
let makingOffer = false;
let ignoreOffer = true;
let polite = false;
let videoToggle = false;
let micToggle = false;
let screenShareToggle = false;
let currentAudioSource = null;
let currentAudioSourceId = null;
let currentVideoSource = null;
let currentVideoSourceId = null;
let videoChange = false;

///UI stuff
let micSources = document.getElementById("micSources");
let videoSources = document.getElementById("videoSources");
let videoBtn = document.querySelector('#videoToggle');
let micBtn = document.querySelector('#micToggle');
let screenShareBtn = document.querySelector('#screenShareToggle');
let recordBtn = document.querySelector('#recordBtn');
let participantCountElement = document.querySelector('#participantCount');
const MediaTypeEnum = Object.freeze({ "Video": 0, "Audio": 1, "Screen": 2 })
let canShare = false;
let videoAvailable = false;
let micAvailable = false;
let recording = false;
let mediaRecorder = null;

let black = ({ width = 1280, height = 720 } = {}) => {
    let canvas = Object.assign(document.createElement("canvas"), { width, height });
    canvas.getContext('2d').fillRect(0, 0, width, height);
    let stream = canvas.captureStream();
    let track = stream.getVideoTracks()[0];
    track.enabled = false;
    return track;
}
populateSources();

///All Signaling functions
async function setupInitialStream() {
    if (videoAvailable) {
        localStream.addTrack(await getStreamWithCurrentVideoSources());
        localStream.getVideoTracks()[0].enabled = false;
    } else {
        //localStream.addTrack(black());
    }
    if (micAvailable) {
        localStream.addTrack(await getStreamWithCurrentAudioSources());
        localStream.getAudioTracks()[0].enabled = false;
    } else {
        //localStream.addTrack(black());
    }
    myvideo.srcObject = localStream;
}


signal.start().then( () => {
    signal.invoke("JoinFaculty", myid, cid).catch((err) => {
        console.log(err);
    });
});

signal.on('liveClassStatus', function (status) {
    console.log(status);
    switch (status) {
        case "Ended":
            signal.stop();
            location = "/Dashboard"
            break;
    }
});

signal.on('joinAcknowledgement', async (message) => {
    if (message == 'Success') {
    }
    if (message == 'ReJoin') {
    }
    //await setupInitialStream();
    $('#stopLive')[0].disabled = false;
});

signal.on('updateStudentList', (userList) => {
    $('#list').empty();
    var count = 0;
    //console.log(userList
    userList.forEach(user => {
        if (user.isAccepted != false) {
            if (user.isAccepted == null) {
                list.innerHTML += createUserItemWithButton(user);
            }
            if (user.isAccepted == true) {
                list.innerHTML += createAcceptedUserItem(user);
                count = + 1;
            }
            participantCountElement.innerHTML = count;
        } else {
            //closeConnection(user.userId);
        }

    });
});

signal.on('receiveSignal', (signalingUser, signal) => {
    newSignal(signalingUser, signal);
});

const sendHubSignal = (message, partnerClientId) => {
    signal.invoke('sendSignal', message, partnerClientId).catch(errorHandler);
};



const newSignal = (partnerClientId, data) => {
    var signal = JSON.parse(data);
    var connection = getConnection(partnerClientId);
    if (signal.sdp) {
        //console.log('WebRTC: sdp signal');
        receivedSdpSignal(partnerClientId, signal.sdp);
    } else if (signal.candidate) {
        //console.log('WebRTC: candidate signal');
        receivedCandidateSignal(partnerClientId, signal.candidate);
    } else {
        console.log('WebRTC: Adding null Candidate');
        connection.addIceCandidate(null);
    }
}

const receivedCandidateSignal = async (partnerClientId, candidate) => {
    var connection = getConnection(partnerClientId);
    var cand = JSON.stringify(candidate);
    console.log(cand);
    try {
        await connection.addIceCandidate(new RTCIceCandidate(candidate));
    } catch (err) {
        if (!ignoreOffer) {
            throw err;
        }
    }
}

const receivedSdpSignal = async (partnerClientId, sdp) => {
    var connection = getConnection(partnerClientId);
    try {
        const offerCollision = (sdp.type == "offer") &&
            (makingOffer || connection.signalingState != "stable");
        ignoreOffer = !polite && offerCollision;
        if (ignoreOffer) {
            return;
        }
        await connection.setRemoteDescription(sdp);
        if (sdp.type == "offer") {
            localStream.getTracks().forEach(track => {
                connection.addTrack(track, localStream);
            });
            await connection.setLocalDescription();
            sendHubSignal(JSON.stringify({ "sdp": connection.localDescription }), partnerClientId);
        }
    } catch (e) {
        console.log(e);
    }
}





//Setup local Video


const getConnection = (partnerClientId) => {
    if (connections[partnerClientId]) {
        console.log("WebRTC: connections partner client exist");
        return connections[partnerClientId];
    }
    else {
        return initializeConnection(partnerClientId);
    }
}

const initializeConnection = (partnerClientId) => {
    var connection = new RTCPeerConnection(peerConnectionConfig);
    connection.onicecandidate = evt => callbackIceCandidate(evt, partnerClientId);
    connection.oniceconnectionstatechange = evt => iceConnectionStateChange(partnerClientId);
    connection.ontrack = evt => callbackAddStream(connection, evt, partnerClientId);
    connection.onnegotiationneeded = evt => callbackNegotiationNeeded(connection, evt, partnerClientId);
    connection.onremovestream = evt => callbackRemoveStream(connection, evt, partnerClientId);
    connections[partnerClientId] = connection;
    return connection;
}



const initiateOffer = async (studentUserId) => {
    var addTracks = connections[studentUserId] == null || connections[studentUserId] == undefined;
    if (Object.size(connections) < 1 && localStream == null) {
        await setupInitialStream();
    }
    var connection = getConnection(studentUserId); // // get a connection for the given partner
    //connection.addStream(stream);// add our audio/video stream
    if (addTracks && localStream != null) {
        localStream.getTracks().forEach(track => {
            connection.addTrack(track, localStream);
        });
    }
    try {
        const offer = await connection.createOffer();
        await connection.setLocalDescription(offer);
        sendHubSignal(JSON.stringify({ "sdp": offer }));
    } catch (e) {
        console.log(e);
    }
}


const callbackIceCandidate = (evt, partnerClientId) => {
    if (evt.candidate) {
        sendHubSignal(JSON.stringify({ "candidate": evt.candidate }), partnerClientId);
    } else {
        sendHubSignal(JSON.stringify({ "": null }), partnerClientId);
    }
}


const iceConnectionStateChange = (partnerClientId) => {
    var connection = getConnection(partnerClientId);
    switch (connection.iceConnectionState) {
        case "failed": connection.restartIce();
            break;
    }
}


const callbackRemoveStream = (connection, evt) => {
}

const callbackAddStream = (connection, evt) => {
}

const callbackNegotiationNeeded = async (connection, evt, partnerClientId) => {
    console.log("WebRTC: Negotiation needed...");
    //console.log(evt);
    try {
        makingOffer = true;
        await connection.setLocalDescription();
        sendHubSignal(JSON.stringify({ "sdp": connection.localDescription }), partnerClientId);
    } catch (err) {
        console.error(err);
    } finally {
        makingOffer = false;
    }
}
const errorHandler = (error) => {
    console.log(error);
};




const closeConnection = (partnerClientId) => {
    console.log("WebRTC: called closeConnection ");
    var connection = connections[partnerClientId];

    if (connection) {
        // Close the connection
        connection.close();
        connection = null;
        connections[partnerClientId] = null;
        delete connections[partnerClientId]; // Remove the property
    }
}
// Close all of our connections
const closeAllConnections = () => {
    Object.keys(connections).forEach(key => {
        closeConnection(key);
    });
}





function populateSources() {
    navigator.mediaDevices.enumerateDevices().then(deviceInfos => {
        for (let i = 0; i !== deviceInfos.length; ++i) {
            const deviceInfo = deviceInfos[i];
            //console.log(deviceInfo);
            const option = document.createElement("button");
            option.classList.add("dropdown-item");
            option.style.width = "100%";
            option.innerText = deviceInfo.label;
            option.dataset.devid = deviceInfo.deviceId;
            if (deviceInfo.kind === 'audioinput') {
                micSources.append(option);
            } else if (deviceInfo.kind === 'videoinput') {
                videoSources.append(option);
            } else {
                //console.log('Some other kind of source/device: ', deviceInfo);
            }
        }
        if (videoSources.childElementCount > 0) {
            videoBtn.disabled = false;
            videoSources.firstElementChild.classList.add("active");
            $('#videoSources').prev()[0].disabled = false;
            $('#fullscreenToggle')[0].disabled = false;
            videoAvailable = true;
            currentVideoSource = videoSources.firstElementChild.innerText;
            currentVideoSourceId = videoSources.firstElementChild.dataset.devid;
            $("#videoSources button").click(function () {
                var selected = $(this)[0];
                if (currentVideoSourceId != selected.dataset.devid) {
                    currentVideoSource = selected.innerText;
                    currentVideoSourceId = selected.dataset.devid;
                    videoSources.querySelector(".active").classList.remove("active");
                    selected.classList.add("active");
                    if (videoToggle) {
                        updateVideoTrack();
                    }
                }
                //console.log(selected);
            })
        }
        if (micSources.childElementCount > 0) {
            micBtn.disabled = false;
            micSources.firstElementChild.classList.add("active");
            $('#micSources').prev()[0].disabled = false;
            micAvailable = true;
            currentAudioSourceId = micSources.firstElementChild.dataset.devid;
            currentAudioSource = micSources.firstElementChild.innerText;
            $("#micSources button").click(function () {
                var selected = $(this)[0];
                if (currentAudioSourceId != selected.dataset.devid) {
                    currentAudioSourceId = selected.dataset.devid;
                    currentAudioSource = selected.innerText;
                    micSources.querySelector(".active").classList.remove("active");
                    selected.classList.add("active");
                    if (micToggle) {
                        updateAudioTrack();
                    }
                }
            })
        }
    }).then(() => {
        setupInitialStream();
    });
    if (navigator.mediaDevices && "getDisplayMedia" in navigator.mediaDevices) {
        screenShareBtn.disabled = false;
        canShare = true;
    }
    if (canShare || videoAvailable || micAvailable) {
        recordBtn.disabled = false;
    }
}


async function getStreamWithCurrentVideoSources() {
    try {
        const constraints = {
            video: { deviceId: currentVideoSourceId ? { exact: currentVideoSourceId } : undefined }
        };
        let stream = await navigator.mediaDevices.getUserMedia(constraints)
        return stream.getVideoTracks()[0];
    } catch (e) {
        return black();
    }
}
async function getStreamWithCurrentAudioSources() {
    try {
        const constraints = {
            audio: { deviceId: currentAudioSourceId ? { exact: currentAudioSourceId } : undefined }
        };
        let stream = await navigator.mediaDevices.getUserMedia(constraints);
        return stream.getAudioTracks()[0];
    } catch (e) {
        //return new MediaStreamTrack();
    }
}

function replaceTrackLocalStream(receivedTrack) {
    if (localStream != null) {
        let track = localStream.getTracks().find(t => t.kind == receivedTrack.kind);
        //track.stop();
        localStream.removeTrack(track);
        localStream.addTrack(receivedTrack);
    }
}

function updateForEachConnection(track) {
    Object.keys(connections).forEach(key => {
        var sender = connections[key].getSenders().find(s => {
            return s.track.kind == track.kind;
        });
        sender.replaceTrack(track);
    });
}

function updateConnectionAndLocalStream(track) {
    updateForEachConnection(track);
    replaceTrackLocalStream(track);
}



//Video Toggle Button
videoBtn.onclick = async () => {
    if (videoToggle) {
        localStream.getVideoTracks()[0].enabled = false;
        videoBtn.classList.replace("btn-danger", "btn-primary");
        videoBtn.innerHTML = '<i data-feather="camera-off"></i> Start Video';
        feather.replace();
        videoToggle = !videoToggle;
    } else {
        if (screenShareToggle) {
            await screenShareBtn.click();
        }
        let track = localStream.getVideoTracks()[0];
        if (currentVideoSource != track.label) {
            await updateVideoTrack();
        } else {
            track.enabled = true;
        }
        videoBtn.classList.replace("btn-primary", "btn-danger");
        videoBtn.innerHTML = '<i data-feather="camera"></i> Stop Video';
        feather.replace();
        videoToggle = !videoToggle;
    }
};
async function updateVideoTrack() {
    const constraints = {
        video: { deviceId: currentVideoSourceId ? { exact: currentVideoSourceId } : undefined }
    };
    try {
        navigator.mediaDevices.getUserMedia(constraints).then(stream => {
            updateConnectionAndLocalStream(stream.getVideoTracks()[0]);
        });
    } catch (e) {
        console.log(e);
        return;
    }
}


//Mic Toggle Button
micBtn.onclick = async () => {
    if (micToggle) {
        localStream.getAudioTracks()[0].enabled = false;
        micBtn.classList.replace("btn-danger", "btn-primary");
        micBtn.innerHTML = '<i data-feather="mic-off"></i> Unmute';
        feather.replace();
        micToggle = !micToggle;
    } else {
        let track = localStream.getAudioTracks()[0];
        if (currentAudioSource != track.label) {
            await updateAudioTrack();
        } else {
            track.enabled = true;
        }
        micBtn.classList.replace("btn-primary", "btn-danger");
        micBtn.innerHTML = '<i data-feather="mic"></i> Mute';
        feather.replace();
        micToggle = !micToggle;
    }
};

async function updateAudioTrack() {
    const constraints = {
        audio: { deviceId: currentAudioSourceId ? { exact: currentAudioSourceId } : undefined }
    };
    try {
        navigator.mediaDevices.getUserMedia(constraints).then(stream => {
            updateConnectionAndLocalStream(stream.getAudioTracks()[0]);
        });
    } catch (e) {
        console.log(e);
        return;
    }
}




var displayMediaOptions = {
    video: {
        cursor: "motion"
    },
    audio: false
};
//Screen Share Toggle
screenShareBtn.onclick = async () => {
    if (screenShareToggle) {
        localStream.getVideoTracks()[0].enabled = false;
        screenShareBtn.classList.replace("btn-danger", "btn-primary");
        screenShareBtn.innerHTML = '<i data-feather="cast"></i> Share Screen';
        feather.replace();
        screenShareToggle = !screenShareToggle;
    } else {
        if (videoToggle) {
            await videoBtn.click();
        }
        try {
            let stream = await navigator.mediaDevices.getDisplayMedia(displayMediaOptions);
            updateConnectionAndLocalStream(stream.getVideoTracks()[0]);
            screenShareBtn.classList.replace("btn-primary", "btn-danger");
            screenShareBtn.innerHTML = '<i data-feather="stop-circle"></i> Stop Screen';
            feather.replace();
            currentVideoSource = "Screen";
            screenShareToggle = !screenShareToggle;
        } catch (e) {
            console.log(e);
        }
    }
};

///Stop Live Streaming Button Click
document.querySelector('#stopLive').onclick = () => {
    closeAllConnections();
    localStream.getTracks().forEach(track => track.stop());
    localStream = null;

    signal.invoke('stopLive', myid, cid)
}

//Full Screen Button Click
document.querySelector('#fullscreenToggle').onclick = () => myvideo.requestFullscreen();
var recordedChunks = [];
function getSupportedMimeTypes() {
    const possibleTypes = [
        'video/webm;codecs=vp9,opus',
        'video/webm;codecs=vp8,opus',
        'video/webm;codecs=h264,opus',
        'video/mp4;codecs=h264,aac',
    ];
    return possibleTypes.filter(mimeType => {
        console.log(MediaRecorder.isTypeSupported(mimeType));
        return MediaRecorder.isTypeSupported(mimeType);
    });
}
function handleDataAvailable(event) {
    console.log("data-available");
    if (event.data.size > 0) {
        recordedChunks.push(event.data);
        console.log(recordedChunks);
        download();
        if (recording) {
            try {
                mediaRecorder.stop();
            } catch (e) {
                console.log(e);
            }
            mediaRecorder = null;
            recordedChunks = [];
            recordBtn.classList.replace("btn-danger", "btn-primary");
            recordBtn.innerHTML = '<i data-feather="play-circle"></i> Record';
            feather.replace();
            recording = !recording;
        }
    } else {
        // ...
    }
}
function download() {
    const mimeType = getSupportedMimeTypes()[0].split(';', 1)[0];
    console.log(mimeType);
    const blob = new Blob(recordedChunks, { type: mimeType });
    var url = URL.createObjectURL(blob);
    var a = document.createElement("a");
    document.body.appendChild(a);
    a.style = "display: none";
    a.href = url;
    var title = $('.card-title')[0].innerText.slice(15)+ " " + Date();
    a.download = title + ".webm";
    a.click();
    window.URL.revokeObjectURL(url);
}



recordBtn.onclick = () => {
    if (recording) {
        try {
            mediaRecorder.stop();
        } catch (e) {
            console.log(e);
        }
        mediaRecorder = null;
        recordedChunks = [];
        recordBtn.classList.replace("btn-danger", "btn-primary");
        recordBtn.innerHTML = '<i data-feather="play-circle"></i> Record';
        feather.replace();
        recording = !recording;
    }
    else {
        if (!mediaRecorder) {
            var options = { mimeType: getSupportedMimeTypes()[0] };
            mediaRecorder = new MediaRecorder(localStream, options);
            mediaRecorder.ondataavailable = handleDataAvailable;
        }
        mediaRecorder.start();
        recordBtn.classList.replace("btn-primary", "btn-danger");
        recordBtn.innerHTML = '<i data-feather="stop-circle"></i> Stop';
        feather.replace();
        recording = !recording;
    }
}

function createAcceptedUserItem(user) {
    initiateOffer(user.userId);
    return `<li data-cid="${user.connectionId}" data-uid="${user.userId}">
        <p>${user.userName}</p>
    </li>`;
}

function createUserItemWithButton(user) {
    return `<li data-cid="${user.connectionId}" data-uid="${user.userId}">
        <p>${user.userName} wants to join</p>
        <button class="btn btn-success waves-effect waves-float waves-light" onclick="accept(\'${user.userId}\')">Accept</button>
        <button class="btn btn-danger waves-effect waves-float waves-light" onclick="reject(\'${user.userId}\')">Reject</button>
    </li>`
}

function accept(sid) {
    signal.invoke('Accept', sid, myid, cid);
}
function reject(sid) {
    signal.invoke('Reject', sid, myid, cid);
}





//Additional JS
Object.size = function (obj) {
    var size = 0,
        key;
    for (key in obj) {
        if (obj.hasOwnProperty(key)) size++;
    }
    return size;
};