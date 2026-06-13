import { useEffect, useState } from "react";

import {
    getNotifications
} from "../../api/NotificationApi";

import type { Notification } from "../../types/notification";
import NotificationDetails from "./NotificationDetails";
import "./NotificationBell.css";

export default function NotificationBell() {

    const [notifications, setNotifications] = useState<Notification[]>([]);
    const [isOpen, setIsOpen] = useState(false);
    const [loading, setLoading] = useState(true);

    async function loadNotifications() {
        try {
            const data = await getNotifications();
            setNotifications(data);
        }
        catch (error) {
            console.error(error);
        }
        finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadNotifications();
    }, []);

    return (
        <div className="notification-bell">

            <button
                className="notification-button"
                onClick={() =>
                    setIsOpen(!isOpen)}
            >
                🔔

                {notifications.length > 0 && (
                    <span className="notification-badge">
                        {notifications.length}
                    </span>
                )}
            </button>

            {isOpen && (
                <div className="notification-dropdown">

                    <h4>
                        Notifications
                    </h4>

                    {loading && (
                        <p>Loading...</p>
                    )}

                    {!loading &&
                        notifications.length === 0 && (
                            <p>
                                No notifications
                            </p>
                    )}

                    {notifications.map(
                        notification => (
                            <NotificationDetails 
                                key={notification.id}
                                notification={notification}
                                onActionCompleted={loadNotifications}
                            />
                        )
                    )}
                </div>
            )}
        </div>
    );
}