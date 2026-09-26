import React from 'react';
import { ExternalLink, Shield } from 'lucide-react';
import { useAuth } from '../auth/AuthContext';
import { appConfig } from '../runtimeConfig';

export const SecurityPage: React.FC = () => {
  const { user } = useAuth();
  const accountUrl = `${appConfig.authAuthority.replace(/\/$/, '')}/account`;

  return (
    <div className="max-w-2xl">
      <div className="mb-6 flex items-center gap-3">
        <Shield className="text-blue-600" size={28} />
        <div>
          <h1 className="text-2xl font-bold">Security</h1>
          <p className="text-sm text-gray-500">Manage your sign-in and account security.</p>
        </div>
      </div>
      <section className="card space-y-4 p-5">
        <div>
          <h2 className="font-semibold text-gray-800">Identity provider</h2>
          <p className="mt-1 text-sm text-gray-600">
            Password changes, two-factor authentication, and active sessions are managed by your identity provider.
          </p>
        </div>
        <p className="text-sm text-gray-600">Signed in as {user?.email ?? user?.username}.</p>
        <a
          href={accountUrl}
          target="_blank"
          rel="noreferrer"
          className="inline-flex items-center gap-2 rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
        >
          Open security settings
          <ExternalLink size={16} aria-hidden="true" />
        </a>
      </section>
    </div>
  );
};
