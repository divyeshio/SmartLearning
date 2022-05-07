'use strict';
// Create SignalRConnection
let hubUrl = '/liveHub';
let signal = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl, signalR.HttpTransportType.WebSockets)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.None).build();

let peerConnectionConfig = {
    iceServers: [
        {
            urls: ["stun:stun.l.google.com:19302","stun:stun1.l.google.com:19302", "stun:stun2.l.google.com:19302","stun:stun3.l.google.com:19302", "stun:stun4.l.google.com:19302"]
        },
        {
        urls: "turn:numb.viagenie.ca",
        credential: 'muazkh',
        username: 'webrtc@live.com'
        }
    ]
};

var localStream = null;
var webrtcConstraints = { audio: true, video: true };
let pc = new RTCPeerConnection(peerConnectionConfig);
let facultyid = document.querySelector("#liveclass").dataset.bid;
let cid = document.querySelector("#liveclass").dataset.cid;
let stdid = document.querySelector("#liveclass").dataset.uid;
let makingOffer = false;
let ignoreOffer = false;
let polite = true;
let fullScreenBtn = document.querySelector('#fullscreenToggle');
const remoteVideo = document.querySelector('#othervideo');


signal.start().then(() => {
    signal.invoke("JoinClass", stdid,cid ).catch((err) => {
        console.log(err);
    });
});

signal.on('liveClassStatus', function (status) {
    console.log(status);
    switch (status) {
        case "Rejected": break;

        case "Wait": $.blockUI({
            message:
                '<div class="d-flex justify-content-center align-items-center"><p class="mr-50 mb-0">Wait for Approval</p> <div class="spinner-grow spinner-grow-sm text-white" role="status"></div> </div>',
            css: {
                backgroundColor: 'transparent',
                color: '#fff',
                border: '0'
            },
            overlayCSS: {
                opacity: 0.5
            }
        });
            break;
        case "Approved": 
            initialize();
            break;

        case "Ended":
            pc.close();
            pc = null;
            location = "/Chat"
            break;
        case "Online":
            //initialize();
            break;
        case "Offline":
            //pc.close();
            //pc = null;
            break;
    }
});




const sendHubSignal = (message) => {
    signal.invoke('sendSignal', message, facultyid).catch(errorHandler);
};

const receivedCandidateSignal = async (candidate) => {
    var cand = JSON.stringify(candidate);
    console.log(cand);
    try {
        await pc.addIceCandidate(new RTCIceCandidate(candidate));
    } catch (err) {
        if (!ignoreOffer) {
            throw err;
        }
    }
}

const receivedSdpSignal = async (partnerClientId, sdp) => {
    try {
        const offerCollision = (sdp.type == "offer") &&
            (makingOffer || pc.signalingState != "stable");
        ignoreOffer = !polite && offerCollision;
        if (ignoreOffer) {
            return;
        }
        await pc.setRemoteDescription(sdp);
        if (sdp.type == "offer") {
            await pc.setLocalDescription();
            sendHubSignal(JSON.stringify({ "sdp": pc.localDescription }));
        }
    } catch (e) {
        console.log(e);
    }
}



async function initialize() {
    remoteVideo.srcObject = new MediaStream();
    const offer = await pc.createOffer();
    await pc.setLocalDescription(offer);
    sendHubSignal(JSON.stringify({ "sdp": offer }));
}

signal.on('receiveSignal', (signalingUser, data) => {
    var signal = JSON.parse(data);
    if (signal.sdp) {
        receivedSdpSignal(signalingUser, signal.sdp);
    } else if (signal.candidate) {
        //console.log('WebRTC: candidate signal');
        receivedCandidateSignal(signal.candidate);
    } else {
        console.log('WebRTC: Adding null Candidate');
        pc.addIceCandidate(null);
    }
});

pc.onicecandidate = (evt, partnerClientId) => {
    if (evt.candidate) {
        sendHubSignal(JSON.stringify({ "candidate": evt.candidate }), partnerClientId);
    } else {
        // Null candidate means we are done collecting candidates.
        console.log('WebRTC: ICE candidate gathering complete');
        sendHubSignal(JSON.stringify({ "": null }), partnerClientId);
    }
}

pc.oniceconnectionstatechange = event => {
    switch (pc.iceConnectionState) {
        case "failed": pc.restartIce();
            break;
    }
}


pc.onconnectionstatechange = event => {
    switch (pc.connectionState) {
        case "connected": $.unblockUI();
            fullScreenBtn.disabled = false;
            break;
    }
}

pc.ontrack = ev => {
    console.log("Track receieved");
    console.log(ev);
    if (ev.streams && ev.streams[0]) {
        $().click();
        remoteVideo.srcObject = ev.streams[0];
        if (remoteVideo.hasAttribute("muted")) {
        $().click();
        setTimeout(() => {
        remoteVideo.muted = false;
        remoteVideo.removeAttribute("muted");
        }, 3000);
        }
    }
}

pc.onnegotiationneeded = async (evt) => {
    console.log("WebRTC: Negotiation needed...");
    //console.log(evt);
    try {
        makingOffer = true;
        await pc.setLocalDescription();
        sendHubSignal(JSON.stringify({ "sdp": pc.localDescription }), facultyid);
    } catch (err) {
        console.error(err);
    } finally {
        makingOffer = false;
    }
}


const errorHandler = (error) => {
    console.log(error);
};



fullScreenBtn.onclick = () => remoteVideo.requestFullscreen();