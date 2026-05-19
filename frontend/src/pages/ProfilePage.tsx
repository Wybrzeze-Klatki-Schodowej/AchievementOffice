import AchievementList from "../components/achievements/AchievementList";
import { useOutletContext, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import type { UserProfile } from "../types/user";
import { getUserProfile } from "../api/UserApi";

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
        <div>
            <div
                style={{
                    maxWidth: "700px",
                    margin: "0 auto",
                    padding: "24px",
                }}
            >
                <h1>User profile</h1>

                <h1>
                    {user.firstName}
                    {" "}
                    {user.lastName}
                </h1>

                <p>
                    @{user.login}
                </p>

                <p>
                    {user.email}
                </p>

                <p>
                    {user.jobTitle}
                </p>

                <p>
                    {user.bio}
                </p>

                <p>
                    User since: {new Date(user.createdAt).toLocaleDateString()}
                </p>

                <AchievementList
                    userId={userId!}
                    refreshTrigger={refreshTrigger}
                />
            </div>
        </div>
    );
}