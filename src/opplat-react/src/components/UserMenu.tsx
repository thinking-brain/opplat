import React, { useEffect, useRef, useState } from 'react';
import { ChevronDown, CreditCard, FileText, LogOut, Shield, UserRound, UsersRound } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { hasAnyRole } from '../auth/roles';
import { appConfig } from '../runtimeConfig';

export const UserMenu: React.FC = () => {
  const { user, logout, roles } = useAuth();
  const [open, setOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);
  const navigate = useNavigate();
  const isTenantAdmin = hasAnyRole(roles, appConfig.accessControl.tenantUserManagementRoles);

  useEffect(() => {
    const closeMenu = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setOpen(false);
      }
    };

    document.addEventListener('mousedown', closeMenu);
    return () => document.removeEventListener('mousedown', closeMenu);
  }, []);

  const goTo = (path: string) => {
    setOpen(false);
    navigate(path);
  };

  return (
    <div ref={menuRef} className="relative">
      <button
        type="button"
        aria-expanded={open}
        aria-haspopup="menu"
        aria-label="User information"
        onClick={() => setOpen(value => !value)}
        className="flex items-center gap-2 rounded px-2 py-1.5 hover:bg-blue-700 transition-colors"
      >
        <span className="flex h-8 w-8 items-center justify-center rounded-full bg-white/20 text-sm font-semibold">
          {(user?.username?.[0] ?? '?').toUpperCase()}
        </span>
        <span className="hidden max-w-32 truncate text-sm sm:block">{user?.username}</span>
        <ChevronDown size={16} aria-hidden="true" />
      </button>

      {open && (
        <div role="menu" className="absolute right-0 z-50 mt-2 w-56 rounded border border-gray-200 bg-white py-1 text-gray-700 shadow-lg">
          <div className="border-b border-gray-100 px-4 py-3">
            <p className="truncate text-sm font-semibold">{user?.username}</p>
            <p className="truncate text-xs text-gray-500">{user?.email}</p>
          </div>
          {isTenantAdmin && (
            <>
              <MenuItem icon={<CreditCard size={16} />} label="Billing" onClick={() => goTo('/billing')} />
              <MenuItem icon={<UsersRound size={16} />} label="Users Management" onClick={() => goTo('/users')} />
              <MenuItem icon={<FileText size={16} />} label="Company Legal Data" onClick={() => goTo('/invoicing/settings')} />
            </>
          )}
          <MenuItem icon={<Shield size={16} />} label="Security" onClick={() => goTo('/account/security')} />
          <MenuItem icon={<UserRound size={16} />} label="User Data" onClick={() => goTo('/account/profile')} />
          <div className="my-1 border-t border-gray-100" />
          <MenuItem icon={<LogOut size={16} />} label="Logout" onClick={() => { setOpen(false); void logout(); }} />
        </div>
      )}
    </div>
  );
};

interface MenuItemProps {
  icon: React.ReactNode;
  label: string;
  onClick: () => void;
}

const MenuItem: React.FC<MenuItemProps> = ({ icon, label, onClick }) => (
  <button
    type="button"
    role="menuitem"
    onClick={onClick}
    className="flex w-full items-center gap-3 px-4 py-2.5 text-left text-sm hover:bg-gray-100"
  >
    {icon}
    <span>{label}</span>
  </button>
);
