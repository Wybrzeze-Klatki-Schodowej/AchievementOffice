import AchievementList from "../components/achievements/AchievementList";
import ShoutoutList from "../components/shoutouts/ShoutoutList.tsx";
import { useOutletContext } from "react-router-dom";

interface OutletContext {
    refreshTrigger: number;
}

export default function ProfilePage() {
    const { refreshTrigger } = useOutletContext<OutletContext>();

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

                <AchievementList 
                    refreshTrigger={refreshTrigger}
                />

                <ShoutoutList 
                    refreshTrigger={refreshTrigger}
                />

            </div>
        </div>
    );
}