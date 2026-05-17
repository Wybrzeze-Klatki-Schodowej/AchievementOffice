import React from 'react';
import { useState } from 'react';
import type { LoginDTO } from '../../api/LoginApi';
import './LoginForm.css';

interface LoadingProps {
    isLoading: boolean;
    onSubmit: (data: LoginDTO) => void;
    error: string | null;
}

export default function LoginForm({ isLoading, onSubmit, error }: LoadingProps) {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit({login, password});

        setPassword('');
    }

    return (
        <form onSubmit={handleSubmit}>
            <input type="text" placeholder='Login' value={login} onChange={(e) => setLogin(e.target.value)} required></input>
            <input type="password" placeholder='Password' value={password} onChange={(e) => setPassword(e.target.value)} required></input>
            {error && <div style={{color: 'red', marginBottom: '1rem'}} className="error-message">{error}</div>}
            <button className="login-button" type="submit" disabled={isLoading}>Login</button>
        </form>
    );
}