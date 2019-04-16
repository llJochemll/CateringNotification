var NotificationTime = /** @class */ (function () {
    function NotificationTime() {
    }
    return NotificationTime;
}());
var Notificationtimes = /** @class */ (function () {
    function Notificationtimes() {
    }
    return Notificationtimes;
}());
var Subscription = /** @class */ (function () {
    function Subscription() {
    }
    return Subscription;
}());
var Verification = /** @class */ (function () {
    function Verification() {
    }
    return Verification;
}());
function getNotificationTime(day) {
    var notificationTime = new NotificationTime();
    var time = document.getElementById(day).value.split(':');
    notificationTime.Hour = +time[0];
    notificationTime.Minute = +time[1];
    return notificationTime;
}
function subscribe() {
    var notificationTimes = new Notificationtimes();
    notificationTimes.Monday = getNotificationTime('monday');
    notificationTimes.Tuesday = getNotificationTime('tuesday');
    notificationTimes.Wednesday = getNotificationTime('wednesday');
    notificationTimes.Thursday = getNotificationTime('thursday');
    notificationTimes.Friday = getNotificationTime('friday');
    var subscription = new Subscription();
    subscription.Email = document.getElementById('email').value;
    subscription.NotificationTimes = notificationTimes;
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST', 'https://pxlcateringnotification.azurewebsites.net/api/subscribe', true);
    xmlhttp.setRequestHeader('Content-type', 'application/json');
    xmlhttp.send(JSON.stringify(subscription));
    xmlhttp.onload = function () {
        if (xmlhttp.status != 200) {
            alert("Oeps, er trad een error op! Weet je zeker dat alle gegevens correct zijn?");
        }
        else {
            window.location.href = "https://cateringstorage.z6.web.core.windows.net/verify?email=" + subscription.Email;
        }
    };
}
function verify() {
    var verification = new Verification();
    verification.Email = document.getElementById('email').value;
    verification.Token = document.getElementById('token').value;
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST', 'https://pxlcateringnotification.azurewebsites.net/api/verify', true);
    xmlhttp.setRequestHeader('Content-type', 'application/json');
    xmlhttp.send(JSON.stringify(verification));
    xmlhttp.onload = function () {
        if (xmlhttp.status != 200) {
            alert("Oeps, er trad een error op! Weet je zeker dat je email en token correct zijn?");
        }
        else {
            alert("Je bevestiging is succesvol doorgegeven!");
        }
    };
}
function getEmailFromURL() {
    var url = new URL(window.location.href);
    var email = url.searchParams.get("email");
    document.getElementById('email').value = email;
}
function checkInput() {
    var expression = /\S+@\S+/;
    if (document.getElementById('token').value.length === 6 && expression.test(String(document.getElementById('email').value).toLowerCase())) {
        document.getElementById('submit').disabled = false;
    }
    else {
        document.getElementById('submit').disabled = true;
    }
}
function checkInputEmail() {
    var expression = /\S+@\S+/;
    if (expression.test(String(document.getElementById('email').value).toLowerCase())) {
        document.getElementById('submit').disabled = false;
    }
    else {
        document.getElementById('submit').disabled = true;
    }
}
