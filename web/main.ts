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
}

function verify() {
    let verification = new Verification();
    verification.Email = (<HTMLInputElement>document.getElementById('email')).value;
    verification.Token = (<HTMLInputElement>document.getElementById('token')).value;


    let xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST', 'https://pxlcateringnotification.azurewebsites.net/api/verify', true);
    xmlhttp.setRequestHeader('Content-type', 'application/json');
    xmlhttp.send(JSON.stringify(verification));
}