<<<<<<< HEAD
=======
import React from 'react';
>>>>>>> main
import LoginForm from '../components/login/LoginForm';
import { useState } from 'react';
import {type LoginDTO, login } from '../api/LoginApi';
import { useNavigate } from 'react-router-dom';
import './LoginPage.css';

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
        <div className='login-page'>
            <div className='text-container'>
                <h1 className='top-left-text'>Share your success</h1>
            </div>
            <div className='form-container'>
                <h2 className='login-title'>Login</h2>
                <LoginForm isLoading={isLoading} onSubmit={handleSubmit} error={error}></LoginForm>
            </div>
        </div>
    );
}