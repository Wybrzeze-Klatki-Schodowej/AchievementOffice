import { useState } from "react";
import { Outlet } from "react-router-dom";
import Navbar from "../components/layout/Navbar";
import AchievementModal from "../components/achievements/AchievementModal";

export default function AppLayout() {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [refreshTrigger, setRefreshTrigger] = useState(0);
    const handleAchievementCreated = () => {
        setRefreshTrigger((prev) => prev + 1);
    };

    return (
        <>
            <Navbar 
                onAddAchievementClick={() =>
                    setIsModalOpen(true)
                }
            />

            <AchievementModal 
                open={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onAchievementCreated={handleAchievementCreated}
            />

            <Outlet 
                context={{ refreshTrigger }}
            />
        </>
    );
}