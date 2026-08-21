import { Component, ElementRef, inject, signal } from '@angular/core';
import { IconButton } from '../../../shared/buttons/icon-button/icon-button';
import { ProfileOptions } from '../profile-options/profile-options';

@Component({
    selector: 'app-profile-button',
    imports: [IconButton, ProfileOptions],
    templateUrl: './profile-button.html',
    styleUrl: './profile-button.scss',
    host: {
        '(document:click)': 'onDocumentClick($event)',
    },
})
export class ProfileButton {
    isOptionsVisible = signal(false);
    private readonly elementRef = inject(ElementRef);

    toggleOptionsVisibility() {
        this.isOptionsVisible.set(!this.isOptionsVisible());
    }

    onDocumentClick(event: MouseEvent): void {
        const target = event.target as Node;

        if (!this.elementRef.nativeElement.contains(target)) {
            this.isOptionsVisible.set(false);
        }
    }

    logout() {
        
    }
}