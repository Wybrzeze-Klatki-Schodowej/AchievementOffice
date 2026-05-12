import { useState } from "react";
import AchievementForm from "../components/achievements/AchievementForm";
import AchievementList from "../components/achievements/AchievementList";

export default function ProfilePage() {
    const [refreshTrigger, setRefreshTrigger] = useState(0);

    const handleAchievementCreated = () => {
        setRefreshTrigger((prev) => prev + 1);
    };

    return (
        <div
            style={{
                maxWidth: "700px",
                margin: "0 auto",
                padding: "24px",
            }}
        >
            <h1>User profile</h1>

            <AchievementForm 
                onAchievementCreated={handleAchievementCreated}
            />

            <hr style={{ margin: "24px 0" }} />

            <AchievementList refreshTrigger={refreshTrigger} />
        </div>
    );
}