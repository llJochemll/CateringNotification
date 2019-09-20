var Campus;
(function (Campus) {
    Campus[Campus["Diepenbeek"] = 0] = "Diepenbeek";
    Campus[Campus["ElfdeLinie"] = 1] = "ElfdeLinie";
    Campus[Campus["Vildersstraat"] = 2] = "Vildersstraat";
})(Campus || (Campus = {}));
var Day;
(function (Day) {
    Day["monday"] = "monday";
    Day["tuesday"] = "tuesday";
    Day["wednesday"] = "wednesday";
    Day["thursday"] = "thursday";
    Day["friday"] = "friday";
})(Day || (Day = {}));
class NotificationTime {
}
class CampusTime {
}
class Subscription {
    constructor() {
        this.NotificationSettings = new Map();
    }
}
const buildSubscription = () => {
    const subscription = new Subscription();
    Object.keys(Day).forEach((day) => {
        if (document.getElementById("no_" + day).checked === false) {
            subscription.NotificationSettings[day] = new CampusTime();
            subscription.NotificationSettings[day].Key = document.getElementById(day + "_location").selectedOptions[0].value;
            subscription.NotificationSettings[day].Value = getNotificationTime(day);
        }
    });
    return subscription;
};
const getNotificationTime = (day) => {
    let notificationTime = new NotificationTime();
    let time = document.getElementById(day).value.split(":");
    notificationTime.Hour = +time[0];
    notificationTime.Minute = +time[1];
    return notificationTime;
};
const subscribe = async () => {
    const email = document.getElementById("email").value;
    const subscription = buildSubscription();
    const response = await fetch(`https://pxlcateringnotification.azurewebsites.net/api/subscribe/${email}`, {
        body: JSON.stringify(subscription),
        headers: {
            "Content-Type": "application/json"
        },
        method: "POST",
    });
    if (response.status != 200) {
        alert(`Oeps, er trad een error op! Weet je zeker dat alle gegevens correct zijn?`);
    }
    else {
        window.location.href = `https://cateringstorage.z6.web.core.windows.net/verify?email=${email}`;
    }
};
const verify = () => {
    const email = document.getElementById("email").value;
    const token = document.getElementById("token").value;
    let xmlhttp = new XMLHttpRequest();
    xmlhttp.open("POST", `https://pxlcateringnotification.azurewebsites.net/api/verify/${email}/${token}`, true);
    xmlhttp.setRequestHeader("Content-type", "application/json");
    xmlhttp.onload = function () {
        if (xmlhttp.status != 200) {
            alert(`Oeps, er trad een error op! Weet je zeker dat je email en token correct zijn?`);
        }
        else {
            alert(`Je bevestiging is succesvol doorgegeven!`);
        }
    };
};
const getEmailFromURL = () => {
    let url = new URL(window.location.href);
    let email = url.searchParams.get("email");
    document.getElementById("email").value = email;
};
const checkTokenInput = () => {
    const expression = /\S+@\S+/;
    if (document.getElementById("token").value.length === 6 && expression.test(String(document.getElementById("email").value).toLowerCase())) {
        document.getElementById("submit").disabled = false;
    }
    else {
        document.getElementById("submit").disabled = true;
    }
};
const checkEmailInput = () => {
    const expression = /\S+@\S+/;
    if (expression.test(String(document.getElementById("email").value).toLowerCase())) {
        document.getElementById("submit").disabled = false;
    }
    else {
        document.getElementById("submit").disabled = true;
    }
};
const no_email_clicked = (day) => {
    const timeElement = document.getElementById(day);
    const locationElement = document.getElementById(day + "_location");
    console.log(timeElement);
    console.log(locationElement);
    if (document.getElementById("no_" + day).checked === true) {
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
};
