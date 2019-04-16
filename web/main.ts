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
    NotificationTimes: Notificationtimes;
}

class Verification {
    Email: string;
    Token: string;
}

function getNotificationTime(day: string): NotificationTime {
    let notificationTime = new NotificationTime();
    let time = (<HTMLInputElement>document.getElementById(day)).value.split(':');
    notificationTime.Hour = +time[0];
    notificationTime.Minute = +time[1];

    return notificationTime;
}

function subscribe() {
    let notificationTimes = new Notificationtimes();
    notificationTimes.Monday = getNotificationTime('monday');
    notificationTimes.Tuesday = getNotificationTime('tuesday');
    notificationTimes.Wednesday = getNotificationTime('wednesday');
    notificationTimes.Thursday = getNotificationTime('thursday');
    notificationTimes.Friday = getNotificationTime('friday');

    let subscription = new Subscription();
    subscription.Email = (<HTMLInputElement>document.getElementById('email')).value;
    subscription.NotificationTimes = notificationTimes;

    let xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST', 'https://pxlcateringnotification.azurewebsites.net/api/subscribe', true);
    xmlhttp.setRequestHeader('Content-type', 'application/json');
    xmlhttp.send(JSON.stringify(subscription));
    xmlhttp.onload = function() {
        if (xmlhttp.status != 200) {
          alert(`Oeps, er trad een error op! Weet je zeker dat alle gegevens correct zijn?`);
        } else {
            window.location.href = "https://cateringstorage.z6.web.core.windows.net/verify?email=" + subscription.Email;
        }
    };
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

function checkInput() {
    const expression = /\S+@\S+/;
    if ((<HTMLInputElement>document.getElementById('token')).value.length === 6 && expression.test(String((<HTMLInputElement>document.getElementById('email')).value).toLowerCase())) {
        (<HTMLInputElement>document.getElementById('submit')).disabled = false;
    } else {
        (<HTMLInputElement>document.getElementById('submit')).disabled = true;
    }
}

function checkInputEmail() {
    const expression = /\S+@\S+/;
    if (expression.test(String((<HTMLInputElement>document.getElementById('email')).value).toLowerCase())) {
        (<HTMLInputElement>document.getElementById('submit')).disabled = false;
    } else {
        (<HTMLInputElement>document.getElementById('submit')).disabled = true;
    }
}
