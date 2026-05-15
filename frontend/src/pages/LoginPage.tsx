import React from 'react';
import LoginForm from '../components/login/LoginForm';
import { useState } from 'react';
import {type LoginDTO, login } from '../api/LoginApi';
import { useNavigate } from 'react-router-dom';

interface LoginPageProps {
    onLogin: () => void;
}

export default function LoginPage({ onLogin }: LoginPageProps) {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleSubmit = async (data: LoginDTO) => {
        setIsLoading(true);
        setError(null);

        try {
            await login(data);
            onLogin();
            navigate('/');
        } catch (err: any) {
            setError(err.message);
        } finally {
            setIsLoading(false);
        }
    }

    return (
        <div>
            {error && <div style={{color: 'red', marginBottom: '1rem'}} className="error-message">{error}</div>}
            <LoginForm isLoading={isLoading} onSubmit={handleSubmit}></LoginForm>
        </div>
    );
}