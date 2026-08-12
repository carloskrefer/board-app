/**
 * Includes methods to help with new reference creation during signal operations to ensure the change is detected.
 */
export class SignalHelper {
    static appendIfNotExistsById<T extends { id: string }>(
        newItem: T,
        items: T[] 
    ): T[] {
        return (items.some(item => item.id === newItem.id)) ?
            items
            : [...items, newItem];
    }

    static updateItemById<T extends { id: string }>(
        id: string, 
        items: T[], 
        update: (item: T) => T
    ): T[] {
        return items.map(item => item.id === id ? 
            { ...update(item) } 
            : item);
    }

    static removeItemById<T extends { id: string }>(
        id: string,
        items: T[], 
    ): T[] {
        return items.filter(item => item.id !== id);
    }
}