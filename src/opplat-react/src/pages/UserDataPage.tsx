import React, { useEffect, useState } from 'react';
import { UserRound } from 'lucide-react';
import { authApi } from '../api/auth.api';
import { useAuth } from '../auth/AuthContext';
import type { User } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

export const UserDataPage: React.FC = () => {
  const { user } = useAuth();
  const [profile, setProfile] = useState<User | null>(null);
  const [name, setName] = useState(user?.name ?? '');
  const [lastName, setLastName] = useState(user?.lastName ?? '');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user?.username) {
      setLoading(false);
      return;
    }

    void authApi.getCurrentUser(user.username)
      .then(data => {
        setProfile(data);
        setName(data.name);
        setLastName(data.lastName);
      })
      .catch(() => setError('Unable to load your profile.'))
      .finally(() => setLoading(false));
  }, [user?.username]);

  const save = async (event: React.FormEvent) => {
    event.preventDefault();
    setSaving(true);
    setMessage(null);
    setError(null);
    try {
      await authApi.updateProfile(name, lastName);
      setMessage('Profile updated. Changes may take effect after the next sign-in.');
      setProfile(current => current ? { ...current, name, lastName } : current);
    } catch {
      setError('Unable to update your profile.');
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="max-w-2xl">
      <div className="mb-6 flex items-center gap-3">
        <UserRound className="text-blue-600" size={28} />
        <div>
          <h1 className="text-2xl font-bold">User Data</h1>
          <p className="text-sm text-gray-500">Review and update your personal information.</p>
        </div>
      </div>
      {message && <div className="mb-4 rounded border border-green-200 bg-green-50 p-3 text-sm text-green-800">{message}</div>}
      {error && <div className="mb-4 rounded border border-red-200 bg-red-50 p-3 text-sm text-red-800">{error}</div>}
      <form onSubmit={save} className="card space-y-4 p-5">
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700" htmlFor="profile-name">Name</label>
          <input id="profile-name" className="input-field" required value={name} onChange={event => setName(event.target.value)} />
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700" htmlFor="profile-last-name">Last name</label>
          <input id="profile-last-name" className="input-field" required value={lastName} onChange={event => setLastName(event.target.value)} />
        </div>
        <div className="grid gap-3 text-sm text-gray-600 sm:grid-cols-2">
          <p><span className="font-medium">Email:</span> {profile?.email ?? user?.email}</p>
          <p><span className="font-medium">Username:</span> {profile?.username ?? user?.username}</p>
          <p><span className="font-medium">Tenant:</span> {user?.tenantIdentifier ?? 'Current tenant'}</p>
          <p><span className="font-medium">Role:</span> {profile?.roles.join(', ') ?? user?.roles.join(', ')}</p>
        </div>
        <button type="submit" disabled={saving} className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50">
          {saving ? 'Saving...' : 'Save changes'}
        </button>
      </form>
    </div>
  );
};
