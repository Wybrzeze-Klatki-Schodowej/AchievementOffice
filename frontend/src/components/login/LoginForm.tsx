import React from 'react';
import { useState } from 'react';
import type { LoginDTO, RegisterDTO } from '../../api/LoginApi';
import './LoginForm.css';

interface AuthFormProps {
    isLoading: boolean;
    mode: 'login' | 'register';
    onSubmit: (data: LoginDTO | RegisterDTO) => void;
    onModeChange: () => void;
    error: string | null;
    successMessage?: string | null;
}

export default function LoginForm({ isLoading, mode, onSubmit, onModeChange, error, successMessage }: AuthFormProps) {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [firstname, setFirstname] = useState('');
    const [lastname, setLastname] = useState('');
    const [jobTitle, setJobTitle] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        if (mode === 'login') {
            onSubmit({ Login: login, Password: password });
            setPassword('');
            return;
        }

        onSubmit({
            Email: email,
            Username: username,
            Password: password,
            Firstname: firstname,
            Lastname: lastname,
            JobTitle: jobTitle,
            RoleName: 'User',
        });
        setPassword('');
    };

    return (
        <form onSubmit={handleSubmit}>
            {mode === 'register' && (
                <>
                    <input type='email' placeholder='Email' value={email} onChange={(e) => setEmail(e.target.value)} required />
                    <input type='text' placeholder='Username' value={username} onChange={(e) => setUsername(e.target.value)} required />
                </>
            )}

            {mode === 'login' && (
                <input type='text' placeholder='Login' value={login} onChange={(e) => setLogin(e.target.value)} required />
            )}

            <input type='password' placeholder='Password' value={password} onChange={(e) => setPassword(e.target.value)} required minLength={8} />

            {mode === 'register' && (
                <>
                    <input type='text' placeholder='First name' value={firstname} onChange={(e) => setFirstname(e.target.value)} required />
                    <input type='text' placeholder='Last name' value={lastname} onChange={(e) => setLastname(e.target.value)} required />
                    <input type='text' placeholder='Job title' value={jobTitle} onChange={(e) => setJobTitle(e.target.value)} required />
                </>
            )}

            {successMessage && <div className='success-message'>{successMessage}</div>}
            {error && <div className='error-message'>{error}</div>}

            <button className='login-button' type='submit' disabled={isLoading}>{mode === 'login' ? 'Login' : 'Register'}</button>

            <div className='toggle-row'>
                {mode === 'login' ? (
                    <span>
                        Don't have an account?{' '}
                        <button type='button' className='toggle-link' onClick={onModeChange}>Register</button>
                    </span>
                ) : (
                    <span>
                        Already have an account?{' '}
                        <button type='button' className='toggle-link' onClick={onModeChange}>Login</button>
                    </span>
                )}
            </div>
        </form>
    );
}