import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import {
    getVerificationRequest,
    approveVerificationRequest,
    rejectVerificationRequest
} from "../api/AchievementVerificationRequestApi";

import type {
    AchievementVerificationRequest
} from "../types/achievementVerificationRequest";

import "./NotificationDetailsPage.css";

export default function NotificationDetailsPage() {

    const { requestId } = useParams();

    const navigate = useNavigate();

    const [request, setRequest] = 
        useState<AchievementVerificationRequest | null>(null);

    const [loading, setLoading] = useState(true);

    const [error, setError] = useState("");

    useEffect(() => {
        loadRequest();
    }, [requestId]);

    async function loadRequest() {

        if (!requestId) {
            return;
        }

        try {
            const data =
                await getVerificationRequest(requestId);

            setRequest(data);
        }
        catch (err) {
            console.error(err);
            setError("Failed to load request");
        }
        finally {
            setLoading(false);
        }
    }

    const handleApprove = async () => {

        if (!request) {
            return;
        }

        try {
            await approveVerificationRequest(
                request.id
            );

            navigate(-1);
        }
        catch (err) {
            console.error(err);
        }
    };

    const handleReject = async () => {

        if (!request) {
            return;
        }

        try {
            await rejectVerificationRequest(
                request.id
            );

            navigate(-1);
        }
        catch (err) {
            console.error(err);
        }
    };

    if (loading) {
        return <p>Loading...</p>;
    }

    if (error) {
        return <p>{error}</p>;
    }

    if (!request) {
        return <p>Request not found</p>;
    }

    return (
        <div className="notification-details-page">

            <h2>
                Achievement Verification Request
            </h2>

            <div className="notification-card">

                <div className="notification-field">
                    <span className="label">
                        Achievement
                    </span>

                    <span>
                        {request.achievementTitle}
                    </span>
                </div>

                <div className="notification-field">
                    <span className="label">
                        Requested by
                    </span>

                    <span>
                        {request.requesterLogin}
                    </span>
                </div>

                <div className="notification-field">
                    <span className="label">
                        Status
                    </span>

                    <span>
                        {request.status}
                    </span>
                </div>

                <div className="notification-field">
                    <span className="label">
                        Created
                    </span>

                    <span>
                        {new Date(
                            request.createdAt
                        ).toLocaleString()}
                    </span>
                </div>

                {request.status === "Pending" && (

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