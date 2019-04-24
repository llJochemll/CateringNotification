class NotificationTime {
    Hour: number;
    Minute: number;
}

class Notificationtimes {
    Monday: NotificationTime;
    Tuesday: NotificationTime;
    Wednesday: NotificationTime;
    Thursday: NotificationTime;
    Friday: NotificationTime;
}

class Subscription {
    Email: string;
    Locations: string[];
    NotificationTimes: Notificationtimes;
}

class Verification {
    Email: string;
    Token: string;
}

enum Days {
    monday = "monday",
    tuesday = "tuesday",
    wednesday = "wednesday",
    thursday = "thursday",
    friday = "friday"
}

function getNotificationTime(day: string): NotificationTime {
    let notificationTime = new NotificationTime();
    let time = (<HTMLInputElement>document.getElementById(day)).value.split(':');
    notificationTime.Hour = +time[0];
    notificationTime.Minute = +time[1];

    return notificationTime;
}

function subscribe() {
    let subscription = new Subscription();
    subscription.Email = (<HTMLInputElement>document.getElementById('email')).value;
    subscription.Locations = getLocations();
    subscription.NotificationTimes = getNotificationsTime();

    let xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST', 'https://pxlcateringnotification.azurewebsites.net/api/subscribe', true);
    xmlhttp.setRequestHeader('Content-type', 'application/json');
    xmlhttp.send(JSON.stringify(subscription));
    xmlhttp.onload = function() {
        if (xmlhttp.status != 200) {
          alert(`Oeps, er trad een error op! Weet je zeker dat alle gegevens correct zijn?`);
        } else 
        {
            // window.location.href = "https://cateringstorage.z6.web.core.windows.net/verify?email=" + subscription.Email;
        }
    };
}

function getNotificationsTime() {
    let notificationTimes = new Notificationtimes();
    Object.keys(Days).forEach((day: string) => {
        if ((<HTMLInputElement>document.getElementById('no_' + day)).checked === false) {
            notificationTimes[day] = getNotificationTime(day);
        }
    });
    return notificationTimes;
}

function verify() {
    let verification = new Verification();
    verification.Email = (<HTMLInputElement>document.getElementById('email')).value;
    verification.Token = (<HTMLInputElement>document.getElementById('token')).value;

    let xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST', 'https://pxlcateringnotification.azurewebsites.net/api/verify', true);
    xmlhttp.setRequestHeader('Content-type', 'application/json');
    xmlhttp.send(JSON.stringify(verification));
    xmlhttp.onload = function() {
        if (xmlhttp.status != 200) {
          alert(`Oeps, er trad een error op! Weet je zeker dat je email en token correct zijn?`);
        } else {
          alert(`Je bevestiging is succesvol doorgegeven!`);
        }
    };
}

function getEmailFromURL() {
    let url = new URL(window.location.href);
    let email = url.searchParams.get("email");
    (<HTMLInputElement>document.getElementById('email')).value = email;
}

function checkTokenInput() {
    const expression = /\S+@\S+/;
    if ((<HTMLInputElement>document.getElementById('token')).value.length === 6 && expression.test(String((<HTMLInputElement>document.getElementById('email')).value).toLowerCase())) {
        (<HTMLInputElement>document.getElementById('submit')).disabled = false;
    } else {
        (<HTMLInputElement>document.getElementById('submit')).disabled = true;
    }
}

function checkEmailInput() {
    const expression = /\S+@\S+/;
    if (expression.test(String((<HTMLInputElement>document.getElementById('email')).value).toLowerCase())) {
        (<HTMLInputElement>document.getElementById('submit')).disabled = false;
    } else {
        (<HTMLInputElement>document.getElementById('submit')).disabled = true;
    }
}

function no_email_clicked(day: string) {
    const timeElement = (<HTMLInputElement>document.getElementById(day));
    const locationElement = (<HTMLSelectElement>document.getElementById(day + "_location"));
    console.log(timeElement);
    console.log(locationElement);
    if ((<HTMLInputElement>document.getElementById('no_' + day)).checked === true) {
        timeElement.value = "--:--";
        timeElement.disabled = true;
        locationElement.value = "";
        locationElement.disabled = true;
    } else {
        timeElement.value = "11:45";
        timeElement.disabled = false;
        locationElement.selectedIndex = 0;
        locationElement.disabled = false;
    }
}

function getLocations() {
    let locations = '{';

    Object.keys(Days).forEach((day: string) => {
        if ((<HTMLSelectElement>document.getElementById(day + "_location")).disabled === false) {
            locations += '"' + day + '": "' + ((<HTMLSelectElement>document.getElementById(day + "_location")).selectedOptions[0].value) + '",';
        }
    });
    locations = locations.substring(0, locations.length - 1) + '}'
    console.log(locations);

    if (locations.length < 5) {
        return JSON.parse('{}');
    }

    return JSON.parse(locations);
}