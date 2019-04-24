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
var Days;
(function (Days) {
    Days["monday"] = "monday";
    Days["tuesday"] = "tuesday";
    Days["wednesday"] = "wednesday";
    Days["thursday"] = "thursday";
    Days["friday"] = "friday";
})(Days || (Days = {}));
function getNotificationTime(day) {
    var notificationTime = new NotificationTime();
    var time = document.getElementById(day).value.split(':');
    notificationTime.Hour = +time[0];
    notificationTime.Minute = +time[1];
    return notificationTime;
}
function subscribe() {
    var subscription = new Subscription();
    subscription.Email = document.getElementById('email').value;
    subscription.Locations = getLocations();
    subscription.NotificationTimes = getNotificationsTime();
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST', 'https://pxlcateringnotification.azurewebsites.net/api/subscribe', true);
    xmlhttp.setRequestHeader('Content-type', 'application/json');
    xmlhttp.send(JSON.stringify(subscription));
    xmlhttp.onload = function () {
        if (xmlhttp.status != 200) {
            alert("Oeps, er trad een error op! Weet je zeker dat alle gegevens correct zijn?");
        }
        else {
            // window.location.href = "https://cateringstorage.z6.web.core.windows.net/verify?email=" + subscription.Email;
        }
    };
}
function getNotificationsTime() {
    var notificationTimes = new Notificationtimes();
    Object.keys(Days).forEach(function (day) {
        if (document.getElementById('no_' + day).checked === false) {
            notificationTimes[day] = getNotificationTime(day);
        }
    });
    return notificationTimes;
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
function checkTokenInput() {
    var expression = /\S+@\S+/;
    if (document.getElementById('token').value.length === 6 && expression.test(String(document.getElementById('email').value).toLowerCase())) {
        document.getElementById('submit').disabled = false;
    }
    else {
        document.getElementById('submit').disabled = true;
    }
}
function checkEmailInput() {
    var expression = /\S+@\S+/;
    if (expression.test(String(document.getElementById('email').value).toLowerCase())) {
        document.getElementById('submit').disabled = false;
    }
    else {
        document.getElementById('submit').disabled = true;
    }
}
function no_email_clicked(day) {
    var timeElement = document.getElementById(day);
    var locationElement = document.getElementById(day + "_location");
    console.log(timeElement);
    console.log(locationElement);
    if (document.getElementById('no_' + day).checked === true) {
        timeElement.value = "--:--";
        timeElement.disabled = true;
        locationElement.value = "";
        locationElement.disabled = true;
    }
    else {
        timeElement.value = "11:45";
        timeElement.disabled = false;
        locationElement.selectedIndex = 0;
        locationElement.disabled = false;
    }
}
function getLocations() {
    var locations = '{';
    Object.keys(Days).forEach(function (day) {
        if (document.getElementById(day + "_location").disabled === false) {
            locations += '"' + day + '": "' + (document.getElementById(day + "_location").selectedOptions[0].value) + '",';
        }
    });
    locations = locations.substring(0, locations.length - 1) + '}';
    console.log(locations);
    if (locations.length < 5) {
        return JSON.parse('{}');
    }
    return JSON.parse(locations);
}
