import { useState } from "react";
import { Outlet } from "react-router-dom";
import Navbar from "../components/layout/Navbar";

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

            <Outlet 
                context={{ refreshTrigger }}
            />
        </>
    );
}