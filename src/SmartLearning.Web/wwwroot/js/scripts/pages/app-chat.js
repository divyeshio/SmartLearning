'use strict';


var chatUsersListWrapper = $('.chat-application .chat-user-list-wrapper'),
    overlay = $('.body-content-overlay'),
    profileSidebar = $('.chat-application .chat-profile-sidebar'),
    profileSidebarArea = $('.chat-application .profile-sidebar-area'),
    profileToggle = $('.chat-application .sidebar-profile-toggle'),
    userProfileToggle = $('.chat-application .user-profile-toggle'),
    userProfileSidebar = $('.user-profile-sidebar'),
    userChats = $('.user-chats'),
    chatsUserList = $('.chat-users-list'),
    chatList = $('.chat-list'),
    contactList = $('.contact-list'),
    sidebarToggle = $('.sidebar-toggle'),
    sidebarContent = $('.sidebar-content'),
    closeIcon = $('.chat-application .close-icon'),
    sidebarCloseIcon = $('.chat-application .sidebar-close-icon'),
    menuToggle = $('.chat-application .menu-toggle'),
    speechToText = $('.speech-to-text'),
    chatSearch = $('.chat-application #chat-search'),
    chatHeadName = $('#chatHeadName'),
    currentChatId = "",
    lastMessagefrom;

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    connection.invoke("GetGroups").then(result => setupChats(result));
}).catch(function (err) {
    return console.error(err.toString());
});

function setupChats(result) {
    for (var i = 0; i < result.length; i++) {
        var html = '<li><span class="avatar"><img src="/images/ico/favicon.ico" height="42" width="42" alt="Generic placeholder image" style="background-color:white"></span><div class="chat-info flex-grow-1"><h5 class="mb-0" data-gid=' + result[i].id + '>' + result[i].name + '</h5><p class="card-text text-truncate">Tap to Open Chat</p></div><div class="chat-meta text-nowrap"><small class="float-right mb-25 chat-time"></small></div></li >';
        chatList.append(html);
    }
    // Add class active on click of Chat users list
    if (chatUsersListWrapper.find('ul li').length) {
        chatUsersListWrapper.find('ul li').on('click', function () {
            var $this = $(this),
                startArea = $('.start-chat-area'),
                activeChat = $('.active-chat'),
                gid = $this.find('h5')[0].dataset.gid;
            if (currentChatId != gid) {
                currentChatId = gid;
                connection.invoke('GetMessageHistory', gid)
                    .then(result => addToChatArea(result));
            }

            if (chatUsersListWrapper.find('ul li').hasClass('active')) {
                chatUsersListWrapper.find('ul li').removeClass('active');
            }
            $this.addClass('active');
            $this.find('.badge').remove();
            chatHeadName[0].innerText = document.querySelector("#users-list > ul.chat-users-list.chat-list.media-list > li.active > div > h5").innerText;

            if (chatUsersListWrapper.find('ul li').hasClass('active')) {
                startArea.addClass('d-none');
                activeChat.removeClass('d-none');
            } else {
                startArea.removeClass('d-none');
                activeChat.addClass('d-none');
            }
        });
    }
    // auto scroll to bottom of Chat area
    chatsUserList.find('li').on('click', function () {
        userChats.animate({ scrollTop: userChats[0].scrollHeight }, 400);
    });

    connection.on("NewMessage", function (message) {
        if (/\S/.test(message)) {
            //check for which group this message is for
            if (message.toClass == currentChatId) {
                if (message.from == lastMessagefrom) {
                    var chatbody = document.querySelector("body > div.app-content.content.chat-application > div.content-area-wrapper > div.content-right > div > div.content-body > section > div > div.user-chats> div.chats > div:last-child > div.chat-body");
                    var element = createChatElementContent(message);
                    chatbody.innerHTML += element;
                } else {
                    if (message.from == $('.chat-user-name')[0].innerText) {
                        var chat = createChatElement(message);
                        $('.chats').append(chat);
                    } else {
                        var chat = createChatElementLeft(message);
                        $('.chats').append(chat);
                    }
                }
                lastMessagefrom = message.from;
                scrollChatToLast();
            } else {
                //This message is for another group. So add a badge
                var element = document.querySelector("#users-list > ul.chat-users-list.chat-list.media-list > li > div > h5[data-gid='" + message.toClass + "']").parentElement.nextElementSibling;
                if (element.childElementCount == 1) {
                    var span = document.createElement("SPAN");
                    span.classList.add("badge", "badge-danger", "badge-pill", "float-right");
                    span.innerText = 1;
                    element.appendChild(span);
                } else {
                    var count = parseInt(element.getElementsByTagName("span")[0].innerText);
                    count += 1;
                    element.getElementsByTagName("span")[0].innerText = count.toString();
                }
                if (Notification.permission === "granted") {
                    // If it's okay let's create a notification
                    new Notification(message.from, {
                        icon: '/images/ico/favicon.ico',
                        body: message.content,
                    });
                }
                // Otherwise, we need to ask the user for permission
                else if (Notification.permission !== "denied") {
                    Notification.requestPermission().then(function (permission) {
                        // If the user accepts, let's create a notification
                        if (permission === "granted") {
                            new Notification(message.from, {
                                icon: '/images/ico/favicon.ico',
                                body: message.content,
                            });
                        }
                    });
                }
            }
        }
    });
}


function scrollChatToLast() {
    $('.user-chats').scrollTop($('.user-chats > .chats').height());
}

async function addToChatArea(messages) {
    clearChat();
    lastMessagefrom = null;
    messages.forEach((message) => {
        console.log(message);   
        if (message.from == lastMessagefrom) {
            var chatbody = document.querySelector("body > div.app-content.content.chat-application > div.content-area-wrapper > div.content-right > div > div.content-body > section > div > div.user-chats> div.chats > div:last-child > div.chat-body");
            var element = createChatElementContent(message);
            chatbody.innerHTML += element;
        } else {
            if (message.from == $('.chat-user-name')[0].innerText) {
                var chat = createChatElement(message);
                $('.chats').append(chat);
            } else {
                var chat = createChatElementLeft(message);
                $('.chats').append(chat);
            }
        }
        lastMessagefrom = message.from;
    });
    scrollChatToLast();
}

function clearChat() {
    $(".chats").empty();
}

function createChatElementContent(message) {
    return '<div class="chat-content"><p>' + message.content + '</p></div>';
}

function createChatElement(message) {
    return '<div class="chat"><div class="chat-avatar">Me</div><div class="chat-body"><div class="chat-content"><p>' + message.content + '</p></div></div></div>';
}
function createChatElementLeft(message) {
    if (message.avatar == null)
        message.avatar = "default.jpg"
    return '<div class="chat chat-left" ><div class="chat-avatar"><span class="avatar box-shadow-1 cursor-pointer"><img src="/StaticFiles/avatars/' + message.avatar + '" alt="avatar" height="36" width="36"></span><span class="m-25 text-primary font-weight-bolder">'+message.fromName +'</span></div><div class="chat-body"><div class="chat-content"><p>' + message.content + '</p></div></div></div>';
}


// init ps if it is not touch device
if (!$.app.menu.is_touch_device()) {
    // Chat user list
    if (chatUsersListWrapper.length > 0) {
        var chatUserList = new PerfectScrollbar(chatUsersListWrapper[0]);
    }

    // Admin profile left
    if (userProfileSidebar.find('.user-profile-sidebar-area').length > 0) {
        var userScrollArea = new PerfectScrollbar(userProfileSidebar.find('.user-profile-sidebar-area')[0]);
    }

    // Chat area
    if (userChats.length > 0) {
        var chatsUser = new PerfectScrollbar(userChats[0], {
            wheelPropagation: false
        });
    }

    // User profile right area
    if (profileSidebarArea.length > 0) {
        var user_profile = new PerfectScrollbar(profileSidebarArea[0]);
    }
} else {
    chatUsersListWrapper.css('overflow', 'scroll');
    userProfileSidebar.find('.user-profile-sidebar-area').css('overflow', 'scroll');
    userChats.css('overflow', 'scroll');
    profileSidebarArea.css('overflow', 'scroll');

    // on user click sidebar close in touch devices
    $(chatsUserList)
        .find('li')
        .on('click', function () {
            $(sidebarContent).removeClass('show');
            $(overlay).removeClass('show');
        });
}

// Chat Profile sidebar & overlay toggle
if (profileToggle.length) {
    profileToggle.on('click', function () {
        profileSidebar.addClass('show');
        overlay.addClass('show');
    });
}



// On Profile close click
if (closeIcon.length) {
    closeIcon.on('click', function () {
        profileSidebar.removeClass('show');
        userProfileSidebar.removeClass('show');
        if (!sidebarContent.hasClass('show')) {
            overlay.removeClass('show');
        }
    });
}

// On sidebar close click
if (sidebarCloseIcon.length) {
    sidebarCloseIcon.on('click', function () {
        sidebarContent.removeClass('show');
        overlay.removeClass('show');
    });
}

// User Profile sidebar toggle
if (userProfileToggle.length) {
    userProfileToggle.on('click', function () {

    });
}

// On overlay click
if (overlay.length) {
    overlay.on('click', function () {
        sidebarContent.removeClass('show');
        overlay.removeClass('show');
        profileSidebar.removeClass('show');
        userProfileSidebar.removeClass('show');
    });
}





// Main menu toggle should hide app menu
if (menuToggle.length) {
    menuToggle.on('click', function (e) {
        sidebarContent.removeClass('show');
        overlay.removeClass('show');
        profileSidebar.removeClass('show');
        userProfileSidebar.removeClass('show');
    });
}

// Chat sidebar toggle
if ($(window).width() < 992) {
    if (sidebarToggle.length) {
        sidebarToggle.on('click', function () {
            sidebarContent.addClass('show');
            overlay.addClass('show');
        });
    }
}

// Filter
if (chatSearch.length) {
    chatSearch.on('keyup', function () {
        var value = $(this).val().toLowerCase();
        if (value !== '') {
            // filter chat list
            chatList.find('li:not(.no-results)').filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
            });
            // filter contact list
            contactList.find('li:not(.no-results)').filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
            });
            var chat_tbl_row = chatList.find('li:not(.no-results):visible').length,
                contact_tbl_row = contactList.find('li:not(.no-results):visible').length;

            // check if chat row available
            if (chat_tbl_row == 0) {
                chatList.find('.no-results').addClass('show');
            } else {
                if (chatList.find('.no-results').hasClass('show')) {
                    chatList.find('.no-results').removeClass('show');
                }
            }

            // check if contact row available
            if (contact_tbl_row == 0) {
                contactList.find('.no-results').addClass('show');
            } else {
                if (contactList.find('.no-results').hasClass('show')) {
                    contactList.find('.no-results').removeClass('show');
                }
            }
        } else {
            // If filter box is empty
            chatsUserList.find('li').show();
            if (chatUsersListWrapper.find('.no-results').hasClass('show')) {
                chatUsersListWrapper.find('.no-results').removeClass('show');
            }
        }
    });
}

if (speechToText.length) {
    // Speech To Text
    var SpeechRecognition = SpeechRecognition || webkitSpeechRecognition;
    if (SpeechRecognition !== undefined && SpeechRecognition !== null) {
        var recognition = new SpeechRecognition(),
            listening = false;
        speechToText.on('click', function () {
            var $this = $(this);
            recognition.onspeechstart = function () {
                listening = true;
            };
            if (listening === false) {
                recognition.start();
            }
            recognition.onerror = function (event) {
                listening = false;
            };
            recognition.onresult = function (event) {
                $this.closest('.form-send-message').find('.message').val(event.results[0][0].transcript);
            };
            recognition.onspeechend = function (event) {
                listening = false;
                recognition.stop();
            };
        });
    }
}
;

// Window Resize
$(window).on('resize', function () {
    if ($(window).width() > 992) {
        if ($('.chat-application .body-content-overlay').hasClass('show')) {
            $('.app-content .sidebar-left').removeClass('show');
            $('.chat-application .body-content-overlay').removeClass('show');
        }
    }

    // Chat sidebar toggle
    if ($(window).width() < 991) {
        if (
            !$('.chat-application .chat-profile-sidebar').hasClass('show') ||
            !$('.chat-application .sidebar-content').hasClass('show')
        ) {
            $('.sidebar-content').removeClass('show');
            $('.body-content-overlay').removeClass('show');
        }
    }
});

// Add message to chat - function call on form submit
function enterChat(source) {
    var message = $('.message').val();
    connection.invoke("SendMessage", currentChatId, message)
    $('.message').val('');

}
