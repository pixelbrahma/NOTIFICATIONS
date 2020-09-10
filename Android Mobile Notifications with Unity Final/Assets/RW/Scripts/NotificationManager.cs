/*
Copyright (c) 2020 Razeware LLC

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom
the Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

Notwithstanding the foregoing, you may not use, copy, modify,
merge, publish, distribute, sublicense, create a derivative work,
and/or sell copies of the Software in any work that is designed,
intended, or marketed for pedagogical or instructional purposes
related to programming, coding, application development, or
information technology. Permission for such use, copying,
modification, merger, publication, distribution, sublicensing,
creation of derivative works, or sale is expressly withheld.

This project and source code may use libraries or frameworks
that are released under various Open-Source licenses. Use of
those libraries and frameworks are governed by their own
individual licenses.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
//1
using System;	//for DateTime
//2
using NotificationSamples;

public class NotificationManager : MonoBehaviour
    {
    //1
	private GameNotificationsManager manager;
	//2
    public const string ChannelId = "game_channel0";
	//3
    public const string ReminderChannelId = "reminder_channel1";
	//4
    public const string NewsChannelId = "news_channel2";

    void Awake() 
    {
        manager = GetComponent<GameNotificationsManager>();
    }

	// Start is called before the first frame update
    void Start()
    {
		//1
        var c1 = new GameNotificationChannel(ChannelId, "Game Alerts", "Game notifications");
		//2
        var c2 = new GameNotificationChannel(NewsChannelId, "News", "News and Events");
		//3
        var c3 = new GameNotificationChannel(ReminderChannelId, "Reminders", "Reminder notifications");
		//4
        manager.Initialize(c1, c2, c3);

        manager.Platform.CancelAllScheduledNotifications();

        ScheduleCurrentNotifications();
    }

	//1
    public void SendNotification(string title, string body, DateTime deliveryTime, int? badgeNumber = null,
								 bool reschedule = false, string channelId = null,
								 string smallIcon = null, string largeIcon = null) 
    {
        //2
		IGameNotification notification = manager.CreateNotification();
		//3
        if (notification == null) 
        {
            return;
        }

		//4
        notification.Title = title;
		//5
        notification.Body = body;
		//6
        notification.Group = !string.IsNullOrEmpty(channelId) ? channelId : ChannelId;
		//7
        notification.DeliveryTime = deliveryTime;
		//8
        notification.SmallIcon = smallIcon;
		//9
        notification.LargeIcon = largeIcon;
        
		//10
		if (badgeNumber != null) 
        {
            notification.BadgeNumber = badgeNumber;
        }

		//11
        PendingNotification notificationToDisplay = manager.ScheduleNotification(notification);
		//12
        notificationToDisplay.Reschedule = reschedule;
    }

	#region UI buttons

	private string smallIconName = "icon_0";
	private string largeIconName = "icon_1";

	//1
	private double shortDelay = 10;

	public void ShortNote() 
	{
		//2
		string title = "Thanks for playing!";
		//3
		string body = "Hope you had fun!";
		//4
		DateTime deliverTime = DateTime.UtcNow.AddSeconds(shortDelay);
		//5
		string channel = ChannelId;
		//6
		SendNotification(title, body, deliverTime, channelId: channel, smallIcon: smallIconName, largeIcon: largeIconName);
	}

	//1
	private double longDelay = 60;

	public void LongNote() 
	{
		//2
		string title = "Play again soon!";
		//3
		string body = "We miss you!";
		//4
		DateTime deliverTime = DateTime.UtcNow.AddSeconds(longDelay);
		//5
		string channel = ChannelId;
		//6
		SendNotification(title, body, deliverTime, channelId: channel, smallIcon: smallIconName, largeIcon: largeIconName);
	}

	#endregion

	private string retentionIconName = "icon_0";
	private string retentionLargeIconName = "icon_1";

	//Schedule notification relative to current game session
	//Move this from seconds to days after testing
	private void RetentionReminderTest(int seconds) 
	{
		string title = "We miss you!";
		string body = string.Concat("You've been away for ", seconds, " seconds");
		DateTime deliverTime = DateTime.UtcNow.AddSeconds(seconds);
		string channel = ReminderChannelId;

		SendNotification(title, body, deliverTime, channelId: channel, smallIcon: retentionIconName, largeIcon: retentionLargeIconName);
	}

	private string eventIconName = "alarm";
	private string eventLargeIconName = "icon_1";

	//Schedule notification to a specific DateTime
	//move this from minutes to a specific DateTime or daysOfTheMonth
	private void EventAlarmTest(int minutesOnTheHour) 
	{
		string title = "Event is Starting!";
		string body = string.Concat("It's ", minutesOnTheHour, " minutes after the hour");
		DateTime deliverTime = GetNextEvent(minutesOnTheHour);
		string channel = ChannelId;

		SendNotification(title, body, deliverTime, channelId: channel, smallIcon: eventIconName, largeIcon: eventLargeIconName);
	}

	private DateTime GetNextEvent(int minutesOnTheHour) 
	{
		DateTime temp = DateTime.Now;

		temp = new DateTime(temp.Year, temp.Month, temp.Day, temp.Hour, minutesOnTheHour, 0);

		return temp;
	}

	void ScheduleCurrentNotifications() 
	{
		//1
		RetentionReminderTest(30);
		//2
		RetentionReminderTest(70);
		//3
		RetentionReminderTest(100);

		//4
		EventAlarmTest(DateTime.Now.Minute + 2);
		//5
		EventAlarmTest(DateTime.Now.Minute + 3);
	}
}
