


let model, ctx, videoWidth, videoHeight, video, canvas;
let captureBtn = document.getElementById('capture');

async function setupCamera() {
    video = document.getElementById('video');
    const stream = await navigator.mediaDevices.getUserMedia({
        'audio': false,
        'video': { facingMode: 'user' },
    });
    video.srcObject = stream;

    return new Promise((resolve) => {
        video.onloadedmetadata = () => {
            resolve(video);
        };
    });
}

var requestSent = false;
const renderPrediction = async () => {
    const returnTensors = false;
    const flipHorizontal = true;
    const annotateBoxes = false;
    const predictions = await model.estimateFaces(
        video, returnTensors, flipHorizontal, annotateBoxes);

    if (predictions.length == 1) {
        if (!requestSent) {
            requestSent = true;
            const canvas1 = document.createElement("canvas");
            canvas1.width = video.videoWidth;
            canvas1.height = video.videoHeight;
            canvas1.getContext('2d')
                .drawImage(video, 0, 0, canvas.width, canvas.height);
            canvas1.toBlob(async function (blob) {
                requestSent = true;
                const formData = new FormData();
                formData.append('FaceImage', blob);
                formData.append('ClassId', document.getElementById('ClassId').value);
                var response = await fetch('/Live/FaceCheck/',
                    {
                        method: 'POST',
                        credentials: 'include',
                        body: formData,
                    })
                //requestSent = false;
                response = await response.json();
                if (response.iserror) {
                    location = location;
                } else {
                    console.log(response.location);
                    location = response.location;
                }
                console.log(response);
            },'image/png',1);
        }
        $('#faceStatus').hide();
        captureBtn.disabled = false;
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        for (let i = 0; i < predictions.length; i++) {
            if (returnTensors) {
                predictions[i].topLeft = predictions[i].topLeft.arraySync();
                predictions[i].bottomRight = predictions[i].bottomRight.arraySync();
                if (annotateBoxes) {
                    predictions[i].landmarks = predictions[i].landmarks.arraySync();
                }
            }

            const start = predictions[i].topLeft;
            const end = predictions[i].bottomRight;
            const size = [end[0] - start[0], end[1] - start[1]];
            ctx.fillStyle = "rgba(255, 0, 0, 0.5)";
            ctx.strokeRect(start[0], start[1], size[0], size[1]);
            ctx.strokeStyle = '#ffff';
            ctx.lineWidth = 3;
            if (annotateBoxes) {
                const landmarks = predictions[i].landmarks;

                ctx.fillStyle = "blue";
                for (let j = 0; j < landmarks.length; j++) {
                    const x = landmarks[j][0];
                    const y = landmarks[j][1];
                    ctx.fillRect(x, y, 5, 5);
                }
            }
        }
    }
    else {
        $('#faceStatus').show();
        //captureBtn.disabled = true;
    }

    requestAnimationFrame(renderPrediction);
};

const setupPage = async () => {
    await setupCamera();
    video.play();

    videoWidth = video.videoWidth;
    videoHeight = video.videoHeight;
    video.width = videoWidth;
    video.height = videoHeight;

    canvas = document.getElementById('output');
    canvas.width = videoWidth;
    canvas.height = videoHeight;
    ctx = canvas.getContext('2d');
    ctx.fillStyle = "rgba(255, 0, 0, 0.5)";

    model = await blazeface.load();

    renderPrediction();
};

setupPage();
captureBtn.disabled = false;

captureBtn.onclick = async () => {
    /*if (!requestSent) {
        requestSent = true;
        const video = document.getElementById("video");

        const canvas1 = document.createElement("canvas");
        canvas1.width = video.videoWidth;
        canvas1.height = video.videoHeight;
        canvas1.getContext('2d')
            .drawImage(video, 0, 0, canvas.width, canvas.height);
        const dataURL = canvas.toDataURL();
        var img = document.createElement("img");
        img.src = dataURL;
        document.body.appendChild(img);
        canvas1.toBlob(function (blob) {
            requestSent = true;
            const formData = new FormData();
            formData.append('FaceImage', blob);
            console.log(blob);
            console.log(formData.get('FaceImage'));
            var response = await fetch('/Home/AddFaceData/',
                {
                    method: 'POST',
                    credentials: 'include',
                    body: formData,
                })
            requestSent = false;
            response = response.json();
            console.log(response);
        });
    }*/
}