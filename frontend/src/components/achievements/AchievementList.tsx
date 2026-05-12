import { useEffect, useState } from "react";
import {
    deleteAchievement,
    getAchievements,
} from "../../api/achievementApi";
import type { Achievement } from "../../types/achievement";
import AchievementCard from "./AchievementCard";

interface Props {
    refreshTrigger: number;
}

export default function AchievementList({
    refreshTrigger,
}: Props) {
    const [achievements, setAchievements] = useState<Achievement[]>([]);
    const [loading, setLoading] = useState(true);

    const loadAchievements = async () => {
        try {
            setLoading(true);

            const data = await getAchievements();

            setAchievements(data);
        } catch (error) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadAchievements();
    }, [refreshTrigger]);

    const handleDelete = async (id: string) => {
        try {
            await deleteAchievement(id);

            await loadAchievements();
        } catch (error) {
            console.error(error);
            alert("Failed to delete achievement");
        }
    };

    if (loading) {
        return <p>Loading achievements...</p>;
    }
    
    return (
        <div>
            <h2>Achievements</h2>

            {achievements.length === 0 && <p>No achievements yet.</p>}

            {achievements.map((achievement) => (
                <div key={achievement.achievementId}>
                    <AchievementCard achievement={achievement} />

                    <button 
                        onClick={() => handleDelete(achievement.achievementId)}
                    >
                        Delete
                    </button>
                </div>
            ))}
        </div>
    );
}