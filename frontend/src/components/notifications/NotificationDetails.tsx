import { useNavigate } from "react-router-dom";

import {
    approveVerificationRequest,
    rejectVerificationRequest
} from "../../api/AchievementVerificationRequestApi";

import type { Notification } from "../../types/notification";
import "./NotificationDetails.css";

interface Props {
    notification: Notification;
    onActionCompleted: () => Promise<void>;
}

export default function NotificationDetails({
    notification,
    onActionCompleted
}: Props) {
    const navigate = useNavigate();

    const handleOpenDetails = () => {

        if (!notification.verificationRequestId) {
            return;
        }

        navigate(
            `/verification-requests/${notification.verificationRequestId}`
        );
    };

    const handleApprove = async (
        e: React.MouseEvent
    ) => {

        e.stopPropagation();

        if (!notification.verificationRequestId) {
            return;
        }

        try {
            await approveVerificationRequest(
                notification.verificationRequestId
            );

            await onActionCompleted();
        }
        catch (error) {
            console.error(error);
        }
    };

    const handleReject = async (
        e: React.MouseEvent
    ) => {

        e.stopPropagation();

        if (!notification.verificationRequestId) {
            return;
        }

        try {
            await rejectVerificationRequest(
                notification.verificationRequestId
            );

            await onActionCompleted();
        }
        catch (error) {
            console.error(error);
        }
    };

    const handleViewAchievement = (
        e: React.MouseEvent
    ) => {

        e.stopPropagation();

        if (!notification.achievementOwnerId) {
            return;
        }

        navigate(
            `/users/${notification.achievementOwnerId}`
        );
    };

    return (
        <div 
            className="notification-item"
            onClick={handleOpenDetails}
        >

            <strong>
                {notification.title}
            </strong>

            <p>
                {notification.message}
            </p>

            {notification.achievementTitle && (
                <div>
                    <b>Achievement:</b>
                    {" "}
                    {notification.achievementTitle}
                </div>
            )}

            {notification.achievementDescription && (
                <div className="notification-meta">
                    <b>Description:</b>
                    {" "}
                    {notification.achievementDescription}
                </div>
            )}

            {notification.achievementOwnerLogin && (
                <div className="notification-meta">
                    <b>Requested by:</b>
                    {" "}
                    {notification.achievementOwnerLogin}
                </div>
            )}

            <div className="notification-footer">
                
                {notification.achievementOwnerId && (
                    <button
                        onClick={handleViewAchievement}
                    >
                        View achievement
                    </button>
                )}

                {notification.verificationRequestId && (
                    <div className="notification-actions">
                        <button
                            onClick={handleApprove}
                        >
                            Approve
                        </button>

                        <button
                            onClick={handleReject}
                        >
                            Deny
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}