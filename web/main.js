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
}
function verify() {
    var verification = new Verification();
    verification.Email = document.getElementById('email').value;
    verification.Token = document.getElementById('token').value;
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST', 'https://pxlcateringnotification.azurewebsites.net/api/verify', true);
    xmlhttp.setRequestHeader('Content-type', 'application/json');
    xmlhttp.send(JSON.stringify(verification));
}
