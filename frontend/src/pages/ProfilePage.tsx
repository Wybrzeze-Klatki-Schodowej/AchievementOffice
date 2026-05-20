import AchievementList from "../components/achievements/AchievementList";
import ShoutoutList from "../components/shoutouts/ShoutoutList.tsx";
import { useOutletContext, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import type { UserProfile } from "../types/user";
import { getUserProfile } from "../api/UserApi";
import "./ProfilePage.css";

interface OutletContext {
    refreshTrigger: number;
}

export default function ProfilePage() {
    const { refreshTrigger } = useOutletContext<OutletContext>();
    const { userId } = useParams();

    const [user, setUser] = useState<UserProfile>();

    useEffect(() => {

        if (!userId) {
            return;
        }

        getUserProfile(userId)
            .then(setUser);
    }, [userId]);

    if (!user) {
        return <p>Loading...</p>;
    }

    return (
        <div className="profile-container">
            <div className="profile-card">
                <div className="profile-header">
                    <div>
                        <h1 className="profile-name">
                            {user.firstName}
                            {" "}
                            {user.lastName}
                        </h1>
                    <div className="profile-username">
                        @{user.login}
                    </div>
                </div>
            </div>

            <div className="profile-meta">
                <div><b>Email:</b> {user.email}</div>
                <div><b>Job Title:</b> {user.jobTitle}</div>
                <div><b>Bio:</b> {user.bio}</div>
                <div>
                    <b>User since:</b>{" "}
                    {new Date(user.createdAt).toLocaleDateString()}
                </div>
            </div>
        </div>

        <div className="profile-card">

            <AchievementList
                userId={userId!}
                refreshTrigger={refreshTrigger}
            />
          
            <ShoutoutList 
                refreshTrigger={refreshTrigger}
            />

        </div>
    </div>
    );
}