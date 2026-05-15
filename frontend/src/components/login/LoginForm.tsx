import React from 'react';
import { useState } from 'react';
import type { LoginDTO } from '../../api/LoginApi';

interface LoadingProps {
    isLoading: boolean;
    onSubmit: (data: LoginDTO) => void;
}

export default function LoginForm({ isLoading, onSubmit }: LoadingProps) {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit({login, password});

        setLogin('');
        setPassword('');
    }

    return (
        <form onSubmit={handleSubmit}>
            <div>
                <label>Login</label>
                <input type="text" value={login} onChange={(e) => setLogin(e.target.value)} required></input>
            </div>
            <div>
                <label>Password:</label>
                <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required></input>
            </div>
            <button type="submit" disabled={isLoading}>Login</button>
        </form>
    );
}