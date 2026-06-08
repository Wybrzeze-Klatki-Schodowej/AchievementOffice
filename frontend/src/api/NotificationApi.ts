import type { Notification } from "../types/notification";

const API_URL = import.meta.env.VITE_API_URL + "/notifications";

export async function getNotifications():
    Promise<Notification[]>
{
    const response = await fetch(API_URL, {
        credentials: "include"
    });

    if (!response.ok) {
        throw new Error("Failed to fetch notifications");
    }

    return response.json();
}

export async function deleteNotification(
    notificationId: string
): Promise<void>
{
    const response = await fetch(
        `${API_URL}/${notificationId}`,
        {
            method: "DELETE",
            credentials: "include"
        });

    if (!response.ok) {
        const error = 
            await response.json().catch(() => null);

        throw new Error(
            error?.message ??
            "Failed to delete notification");
    }
}