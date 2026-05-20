import { useState } from "react";
import { Outlet } from "react-router-dom";
import Navbar from "../components/layout/Navbar";
import AchievementModal from "../components/achievements/AchievementModal";
import ShoutoutModal from "../components/shoutouts/ShoutoutModal";

export default function AppLayout() {
    const [isAchievementModalOpen, setIsAchievementModalOpen] = useState(false);
    const [isShoutoutModalOpen, setIsShoutoutModalOpen] = useState(false);

    const [refreshTrigger, setRefreshTrigger] = useState(0);

    const handleAchievementCreated = () => {
        setRefreshTrigger((prev) => prev + 1);
    };

    const handleShoutoutCreated = () => {
        setRefreshTrigger((prev) => prev + 1);
    };

    return (
        <>
            <Navbar 
                onAddAchievementClick={() =>
                    setIsAchievementModalOpen(true)
                }

                onAddShoutoutClick={() =>
                    setIsShoutoutModalOpen(true)
                }
            />


            <AchievementModal 
                open={isAchievementModalOpen}
                onClose={() => setIsAchievementModalOpen(false)}
                onAchievementCreated={handleAchievementCreated}
            />

            <ShoutoutModal 
                open={isShoutoutModalOpen}
                onClose={() => setIsShoutoutModalOpen(false)}
                onShoutoutCreated={handleShoutoutCreated}
            />

            <Outlet 
                context={{ refreshTrigger }}
            />
        </>
    );
}